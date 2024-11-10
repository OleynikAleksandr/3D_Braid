using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Drawing;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using System.Threading.Tasks;
using Grasshopper.GUI.Base;
using Rhino;

namespace _3D_Braid
{
    public class BraidComponent : GH_Component
    {
        private GH_Document GrasshopperDocument;
        private IGH_Component Component;
        private BraidParameters _parameters;
        private BraidGeometryGenerator _generator;
        private Circle _debugCircle;

        public BraidComponent()
            : base("3D Braid", "Braid", "Создает 3D косичку по окружности с заданными параметрами и сечением профиля", "Oliinyk", "Objects")
        {
            _parameters = new BraidParameters();
            _generator = new BraidGeometryGenerator(_parameters);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("12345678-1234-1234-1234-123456789ABC"); }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            var widthParam = new Grasshopper.Kernel.Parameters.Param_Number
            {
                Name = "Width",
                NickName = "mm",
                Description = "Ширина косички (мм)",
                Access = GH_ParamAccess.item,
                Optional = false
            };
            widthParam.SetPersistentData(6.0);
            pManager.AddParameter(widthParam);

            pManager.AddNumberParameter("Height", "mm", "Высота косички (мм)", GH_ParamAccess.item, 2.0);
            pManager.AddNumberParameter("Steepness", "ST\u200A\u200A\u200A\u200A", "Крутизна косички", GH_ParamAccess.item, 1.0);
            pManager.AddIntegerParameter("Points/Period", "n/n\u200A\u200A\u200A\u200A", "Точки на период", GH_ParamAccess.item, 20);
            pManager.AddNumberParameter("Diameter", "mm", "Диаметр кольца (мм)", GH_ParamAccess.item, 18.0);
            pManager.AddNumberParameter("Diameter Offset", "mm", "Смещение диаметра (мм)", GH_ParamAccess.item, 0.2);
            pManager.AddIntegerParameter("Num Periods", "n/n", "Количество периодов", GH_ParamAccess.item, 14);

            var sectionParam = new Param_Curve
            {
                Name = "Section",
                NickName = "Section",
                Description = "Секционная кривая",
                Access = GH_ParamAccess.item,
                Optional = true
            };
            pManager.AddParameter(sectionParam);
        }

        public override void AddedToDocument(GH_Document document)
        {
            base.AddedToDocument(document);
            Component = this;
            GrasshopperDocument = document;

            // Начинаем последовательное создание элементов управления с задержкой
            Task.Delay(100).ContinueWith(_ => CreateSlidersWithDelay());
        }

        private void CreateSlidersWithDelay()
        {
            GrasshopperDocument.ScheduleSolution(1, doc =>
            {
                CreateSliders(GrasshopperDocument);

                // Задержка перед добавлением ноды Curve
                Task.Delay(50).ContinueWith(__ =>
                {
                    CreateCurveParameter(GrasshopperDocument);

                    // Запускаем расчет геометрии только после добавления всех элементов
                    GrasshopperDocument.ScheduleSolution(1, d => ExpireSolution(true));
                });
            });
        }

        private void CreateSliders(GH_Document document)
        {
            CreateSlider(document, Params.Input[0] as Grasshopper.Kernel.Parameters.Param_Number, "mm", 6.0, BraidParameters.WIDTH_MIN, BraidParameters.WIDTH_MAX);
            CreateSlider(document, Params.Input[1] as Grasshopper.Kernel.Parameters.Param_Number, "mm", 2.0, BraidParameters.HEIGHT_MIN, BraidParameters.HEIGHT_MAX);
            CreateSlider(document, Params.Input[4] as Grasshopper.Kernel.Parameters.Param_Number, "mm", 18.0, BraidParameters.DIAMETER_MIN, BraidParameters.DIAMETER_MAX);
            CreateSlider(document, Params.Input[5] as Grasshopper.Kernel.Parameters.Param_Number, "mm", 0.2, BraidParameters.OFFSET_MIN, BraidParameters.OFFSET_MAX);
            CreateSlider(document, Params.Input[2] as Grasshopper.Kernel.Parameters.Param_Number, "ST", 1.0, BraidParameters.STEEPNESS_MIN, BraidParameters.STEEPNESS_MAX);
            CreateSlider(document, Params.Input[3] as Grasshopper.Kernel.Parameters.Param_Integer, "n/n", 20, BraidParameters.POINTS_MIN, BraidParameters.POINTS_MAX);
            CreateSlider(document, Params.Input[6] as Grasshopper.Kernel.Parameters.Param_Integer, "n/n", 14, BraidParameters.PERIODS_MIN, BraidParameters.PERIODS_MAX);
        }

        private void CreateSlider(GH_Document document, IGH_Param param, string nickName, double defaultValue, double minValue, double maxValue)
        {
            if (param != null)
            {
                var slider = new GH_NumberSlider();
                slider.CreateAttributes();

                slider.Attributes.Pivot = new PointF(
                    (float)this.Attributes.DocObject.Attributes.Bounds.Left - 200,
                    (float)param.Attributes.Bounds.Y
                );

                slider.Name = nickName;
                slider.NickName = nickName;
                slider.Description = param.Description;

                slider.Slider.Type = GH_SliderAccuracy.Float;
                slider.Slider.Minimum = Convert.ToDecimal(minValue);
                slider.Slider.Maximum = Convert.ToDecimal(maxValue);
                slider.Slider.DecimalPlaces = 1;
                slider.Slider.Value = Convert.ToDecimal(defaultValue);

                document.AddObject(slider, false);
                param.AddSource(slider);
            }
        }

        private void CreateCurveParameter(GH_Document document)
        {
            var param = Params.Input[7] as Grasshopper.Kernel.Parameters.Param_Curve;
            if (param != null)
            {
                var curveParam = new Grasshopper.Kernel.Parameters.Param_Curve
                {
                    Name = "Section",
                    NickName = "Section",
                    Description = "Секционная кривая"
                };
                curveParam.CreateAttributes();

                curveParam.Attributes.Pivot = new PointF(
                    (float)this.Params.Input[7].Attributes.Pivot.X - (float)(this.Params.Input[7].Attributes.Bounds.Width),
                    (float)this.Params.Input[7].Attributes.Pivot.Y
                );

                document.AddObject(curveParam, false);
                param.AddSource(curveParam);
            }
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double width = 6.0;
            double height = 0.0;
            double steepness = 0.0;
            int pointsPeriod = 0;
            double diameter = 0.0;
            double diameterOffset = 0.0;
            int numPeriods = 0;
            Curve sectionCurve = null;

            if (!DA.GetData(0, ref width)) return;
            if (!DA.GetData(1, ref height)) return;
            if (!DA.GetData(2, ref steepness)) return;
            if (!DA.GetData(3, ref pointsPeriod)) return;
            if (!DA.GetData(4, ref diameter)) return;
            if (!DA.GetData(5, ref diameterOffset)) return;
            if (!DA.GetData(6, ref numPeriods)) return;

            if (!DA.GetData(7, ref sectionCurve) || sectionCurve == null)
            {
                sectionCurve = new Ellipse(Plane.WorldXY, 0.5, 0.8).ToNurbsCurve();
                Transform rotation = Transform.Rotation(RhinoMath.ToRadians(45), Plane.WorldXY.ZAxis, Point3d.Origin);
                sectionCurve.Transform(rotation);
            }

            // Восстанавливаем отладочную окружность
            _debugCircle = new Circle(new Plane(Point3d.Origin, Vector3d.ZAxis, Vector3d.XAxis), diameter / 2);

            try
            {
                _parameters.Width = width;
                _parameters.Height = height;
                _parameters.Steepness = steepness;
                _parameters.PointsPeriod = pointsPeriod;
                _parameters.Diameter = diameter + height;
                _parameters.DiameterOffset = diameterOffset;
                _parameters.NumPeriods = numPeriods;
                _parameters.SectionCurve = sectionCurve;

                var result = _generator.GenerateBraid();
                if (result != null)
                {
                    DA.SetDataTree(0, result);
                }
            }
            catch (ArgumentException ex)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, ex.Message);
            }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            base.DrawViewportWires(args);

            // Отображаем отладочную окружность
            if (_debugCircle.IsValid)
            {
                args.Display.DrawCircle(_debugCircle, Color.Black, 4); // Черный цвет и толщина 4 пикселя
            }
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Breps", "B", "Результирующая геометрия", GH_ParamAccess.tree);
        }
    }
}

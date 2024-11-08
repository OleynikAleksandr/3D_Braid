using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Drawing;
using Grasshopper.Kernel.Parameters;
using System.Linq;
using Grasshopper.Kernel.Special;
using Grasshopper.GUI.Base;
using System.Threading;

namespace _3D_Braid
{
    public class BraidComponent : GH_Component
    {
        private GH_Document GrasshopperDocument;
        private IGH_Component Component;
        private BraidParameters _parameters;
        private BraidGeometryGenerator _generator;
        private bool _sliderCreated = false;

        public BraidParameters Parameters
        {
            get { return _parameters; }
        }

        public BraidComponent()
            : base("3D Braid",
                   "Braid",
                   "Создает 3D косичку по окружности с заданными параметрами и сечением профиля",
                   "Oliinyk",
                   "Objects")
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
                NickName = "Width",
                Description = "Ширина косички (мм)",
                Access = GH_ParamAccess.item,
                Optional = false
            };
            widthParam.SetPersistentData(6.0);
            pManager.AddParameter(widthParam);

            pManager.AddNumberParameter("Height", "H", "Высота косички (мм)", GH_ParamAccess.item, 2.0);
            pManager.AddNumberParameter("Steepness", "S", "Крутизна косички (мм)", GH_ParamAccess.item, 1.0);
            pManager.AddIntegerParameter("Points/Period", "Pts", "Точки на период", GH_ParamAccess.item, 20);
            pManager.AddNumberParameter("Diameter", "D", "Диаметр кольца (мм)", GH_ParamAccess.item, 18.0);
            pManager.AddNumberParameter("Diameter Offset", "DO", "Смещение диаметра (мм)", GH_ParamAccess.item, 0.2);
            pManager.AddIntegerParameter("Num Periods", "N", "Количество периодов", GH_ParamAccess.item, 14);
            pManager.AddCurveParameter("Section", "Sec", "Секционная кривая", GH_ParamAccess.item);
        }

        public override void AddedToDocument(GH_Document document)
        {
            base.AddedToDocument(document);

            Component = this;
            GrasshopperDocument = document; // Используем переданный документ

            // Запускаем таймер для отложенного создания слайдера
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // 1 секунда
            timer.Tick += (sender, e) =>
            {
                if (!_sliderCreated && this.Attributes != null && this.Params.Input[0].Attributes != null)
                {
                    CreateSlider(document);
                    _sliderCreated = true;
                    timer.Stop();
                    timer.Dispose();
                }
            };
            timer.Start();
        }

        private void CreateSlider(GH_Document document)
        {
            Grasshopper.Kernel.Parameters.Param_Number param = Params.Input[0] as Grasshopper.Kernel.Parameters.Param_Number;
            if (param != null)
            {
                var slider = new GH_NumberSlider();
                slider.CreateAttributes();

                // Создаем и позиционируем слайдер
                if (this.Attributes != null && this.Attributes.DocObject != null)
                {
                    try
                    {
                        slider.Attributes.Pivot = new PointF(
                            (float)this.Attributes.DocObject.Attributes.Bounds.Left - 200,
                            (float)param.Attributes.Bounds.Y
                        );

                        // Настраиваем слайдер
                        slider.Name = "Width (мм)";
                        slider.NickName = "Width";
                        slider.Description = "Ширина косички (мм)";

                        if (slider.Slider != null)
                        {
                            slider.Slider.Type = GH_SliderAccuracy.Float;
                            slider.Slider.Minimum = Convert.ToDecimal(BraidParameters.WIDTH_MIN);
                            slider.Slider.Maximum = Convert.ToDecimal(BraidParameters.WIDTH_MAX);
                            slider.Slider.DecimalPlaces = 1;
                            slider.Slider.Value = Convert.ToDecimal(6.0);
                            slider.Slider.GripDisplay = GH_SliderGripDisplay.ShapeAndText;
                        }

                        // Добавляем на канвас
                        document.AddObject(slider, false);
                        param.AddSource(slider);
                        document.NewSolution(true);
                    }
                    catch (Exception ex)
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Ошибка создания слайдера: " + ex.Message);
                    }
                }
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
            if (!DA.GetData(7, ref sectionCurve)) return;

            try
            {
                _parameters.Width = width;
                _parameters.Height = height;
                _parameters.Steepness = steepness;
                _parameters.PointsPeriod = pointsPeriod;
                _parameters.Diameter = diameter;
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

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Breps", "B", "Результирующая геометрия", GH_ParamAccess.tree);
        }
    }
}
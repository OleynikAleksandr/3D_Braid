using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Drawing;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using System.Threading.Tasks;
using Grasshopper.GUI.Base;
using Rhino;
using System.Collections.Generic;

namespace _3D_Braid
{
    public class BraidComponent : GH_Component
    {
        private GH_Document GrasshopperDocument;
        private IGH_Component Component;
        private BraidParameters _parameters;
        private BraidGeometryGenerator _generator;
        private bool _slidersCreated = false;
        private bool _curveCreated = false;
        private Circle _debugCircle;
        private Dictionary<int, GH_NumberSlider> _sliders;

        public BraidParameters Parameters { get { return _parameters; } }

        public BraidComponent()
            : base("3D Braid",
                  "Braid",
                  "Создает 3D косичку по окружности с заданными параметрами и сечением профиля",
                  "Oliinyk",
                  "Objects")
        {
            _parameters = new BraidParameters();
            _generator = new BraidGeometryGenerator(_parameters);
            _sliders = new Dictionary<int, GH_NumberSlider>();
        }

        public override Guid ComponentGuid => new Guid("12345678-1234-1234-1234-123456789ABC");

        public override void CreateAttributes()
        {
            m_attributes = new BraidComponentAttributes(this);
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            var widthParam = new FollowingParam
            {
                Name = "Width",
                NickName = "mm",
                Description = "Ширина косички (мм)",
                Access = GH_ParamAccess.item,
                Optional = false
            };
            widthParam.SetPersistentData(6.0);
            pManager.AddParameter(widthParam);

            var heightParam = new FollowingParam
            {
                Name = "Height",
                NickName = "mm",
                Description = "Высота косички (мм)",
                Access = GH_ParamAccess.item
            };
            heightParam.SetPersistentData(2.0);
            pManager.AddParameter(heightParam);

            var steepParam = new FollowingParam
            {
                Name = "Steepness",
                NickName = "ST",
                Description = "Крутизна косички",
                Access = GH_ParamAccess.item
            };
            steepParam.SetPersistentData(1.0);
            pManager.AddParameter(steepParam);

            var pointsParam = new FollowingParam
            {
                Name = "Points/Period",
                NickName = "n/n",
                Description = "Точки на период",
                Access = GH_ParamAccess.item
            };
            pointsParam.SetPersistentData(20);
            pManager.AddParameter(pointsParam);

            var diameterParam = new FollowingParam
            {
                Name = "Diameter",
                NickName = "mm",
                Description = "Диаметр кольца (мм)",
                Access = GH_ParamAccess.item
            };
            diameterParam.SetPersistentData(18.0);
            pManager.AddParameter(diameterParam);

            var offsetParam = new FollowingParam
            {
                Name = "Diameter Offset",
                NickName = "mm",
                Description = "Смещение диаметра (мм)",
                Access = GH_ParamAccess.item
            };
            offsetParam.SetPersistentData(0.2);
            pManager.AddParameter(offsetParam);

            var periodsParam = new FollowingParam
            {
                Name = "Num Periods",
                NickName = "n/n",
                Description = "Количество периодов",
                Access = GH_ParamAccess.item
            };
            periodsParam.SetPersistentData(14);
            pManager.AddParameter(periodsParam);

            var sectionParam = new FollowingCurveParam
            {
                Name = "Section",
                NickName = "Section",
                Description = "Секционная кривая",
                Access = GH_ParamAccess.item,
                Optional = true
            };
            pManager.AddParameter(sectionParam);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Breps", "B", "Результирующая геометрия", GH_ParamAccess.tree);
        }
        public override void AddedToDocument(GH_Document document)
        {
            base.AddedToDocument(document);

            Component = this;
            GrasshopperDocument = document;

            Task.Delay(100).ContinueWith(_ =>
            {
                GrasshopperDocument.ScheduleSolution(1, doc =>
                {
                    if (!_slidersCreated && this.Attributes != null && this.Params.Input[0].Attributes != null)
                    {
                        CreateSliders(GrasshopperDocument);
                        _slidersCreated = true;
                    }

                    if (!_curveCreated && this.Attributes != null && this.Params.Input[7].Attributes != null)
                    {
                        CreateCurveParameter(GrasshopperDocument);
                        _curveCreated = true;
                    }

                    if (!_slidersCreated || !_curveCreated)
                    {
                        ExpireSolution(true);
                    }
                });
            });
        }

        private void CreateSliders(GH_Document document)
        {
            try
            {
                float sliderOffset = 200;
                CreateSlider(document, 0, "mm", 6.0, BraidParameters.WIDTH_MIN, BraidParameters.WIDTH_MAX, sliderOffset, GH_SliderAccuracy.Float);
                CreateSlider(document, 1, "mm", 2.0, BraidParameters.HEIGHT_MIN, BraidParameters.HEIGHT_MAX, sliderOffset, GH_SliderAccuracy.Float);
                CreateSlider(document, 4, "mm", 18.0, BraidParameters.DIAMETER_MIN, BraidParameters.DIAMETER_MAX, sliderOffset, GH_SliderAccuracy.Float);
                CreateSlider(document, 5, "mm", 0.2, BraidParameters.OFFSET_MIN, BraidParameters.OFFSET_MAX, sliderOffset, GH_SliderAccuracy.Float);
                CreateSlider(document, 2, "ST", 1.0, BraidParameters.STEEPNESS_MIN, BraidParameters.STEEPNESS_MAX, sliderOffset, GH_SliderAccuracy.Float);
                CreateSlider(document, 3, "n/n", 20.0, BraidParameters.POINTS_MIN, BraidParameters.POINTS_MAX, sliderOffset, GH_SliderAccuracy.Integer);
                CreateSlider(document, 6, "n/n", 14.0, BraidParameters.PERIODS_MIN, BraidParameters.PERIODS_MAX, sliderOffset, GH_SliderAccuracy.Integer);
            }
            catch { }
        }

        private void CreateSlider(GH_Document document, int paramIndex, string nickName, double defaultValue, double minValue, double maxValue, float sliderOffset, GH_SliderAccuracy accuracy)
        {
            var param = Params.Input[paramIndex];
            if (param is null) return;

            var slider = new GH_NumberSlider();
            slider.CreateAttributes();

            if (this.Attributes?.DocObject?.Attributes != null)
            {
                slider.Attributes.Pivot = new PointF(
                    (float)this.Attributes.DocObject.Attributes.Bounds.Left - sliderOffset,
                    (float)param.Attributes.Bounds.Y
                );

                slider.Name = nickName;
                slider.NickName = nickName;
                slider.Description = param.Description;

                if (slider.Slider != null)
                {
                    slider.Slider.Type = accuracy;
                    slider.Slider.DecimalPlaces = (accuracy == GH_SliderAccuracy.Integer) ? 0 : 1;
                    slider.Slider.Minimum = Convert.ToDecimal(minValue);
                    slider.Slider.Maximum = Convert.ToDecimal(maxValue);
                    slider.Slider.Value = Convert.ToDecimal(defaultValue);

                    // Устанавливаем начальные цвета в зависимости от состояния Follow
                    if (param is FollowingParam followingParam)
                    {
                        UpdateSliderColors(slider, followingParam.IsFollowing);
                    }
                }

                document.AddObject(slider, false);
                param.AddSource(slider);
                _sliders[paramIndex] = slider;
            }
        }

        private void CreateCurveParameter(GH_Document document)
        {
            if (Params.Input[7] is Param_Curve param)
            {
                var curveParam = new Param_Curve();
                curveParam.CreateAttributes();

                if (this.Attributes != null && this.Attributes.DocObject != null)
                {
                    curveParam.Attributes.Pivot = new PointF(
                        (float)this.Params.Input[7].Attributes.Pivot.X - (float)(this.Params.Input[7].Attributes.Bounds.Width) - 25,
                        (float)this.Params.Input[7].Attributes.Pivot.Y + 17
                    );

                    curveParam.Name = "Section";
                    curveParam.NickName = "Section";
                    curveParam.Description = "Секционная кривая";

                    document.AddObject(curveParam, false);
                    param.AddSource(curveParam);
                }
            }
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double width = 6.0;
            double height = 0.0;
            double steepness = 0.0;
            double pointsPeriodDouble = 0.0;
            double diameter = 0.0;
            double diameterOffset = 0.0;
            double numPeriodsDouble = 0.0;
            Curve sectionCurve = null;

            if (!DA.GetData(0, ref width)) return;
            if (!DA.GetData(1, ref height)) return;
            if (!DA.GetData(2, ref steepness)) return;
            if (!DA.GetData(3, ref pointsPeriodDouble)) return;
            if (!DA.GetData(4, ref diameter)) return;
            if (!DA.GetData(5, ref diameterOffset)) return;
            if (!DA.GetData(6, ref numPeriodsDouble)) return;

            // Преобразуем в целые числа
            int pointsPeriod = (int)Math.Round(pointsPeriodDouble);
            int numPeriods = (int)Math.Round(numPeriodsDouble);

            if (!DA.GetData(7, ref sectionCurve) || sectionCurve == null)
            {
                sectionCurve = new Ellipse(Plane.WorldXY, 0.5, 0.8).ToNurbsCurve();
                Transform rotation = Transform.Rotation(RhinoMath.ToRadians(45), Plane.WorldXY.ZAxis, Point3d.Origin);
                sectionCurve.Transform(rotation);
            }

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

            if (_debugCircle.IsValid)
            {
                args.Display.DrawCircle(_debugCircle, Color.Black, 4);
            }
        }

        public bool ShouldParameterFollow(IGH_Param param)
        {
            if (param is FollowingParam followingParam)
            {
                return followingParam.IsFollowing;
            }
            return false;
        }

        // ДОБАВЛЯЕМ СЮДА новый метод:
        public void UpdateSliderColors(GH_NumberSlider slider, bool isFollowing)
        {
            if (slider?.Slider != null)
            {
                if (isFollowing)
                {
                    slider.Slider.GripTopColour = Color.FromArgb(0, 120, 255);    // Более яркий синий
                    slider.Slider.GripBottomColour = Color.FromArgb(0, 120, 255); // Более яркий синий
                    slider.Slider.GripEdgeColour = Color.Black;                   // Черный для контура
                    slider.Slider.TextColour = Color.FromArgb(255, 0, 0);         // Красный для текста
                }
                else
                {
                    // Возвращаем цвета по умолчанию
                    slider.Slider.GripTopColour = Color.White;
                    slider.Slider.GripBottomColour = Color.White;
                    slider.Slider.GripEdgeColour = Color.Black;
                    slider.Slider.TextColour = Color.Black;
                }
            }
        }
    }
}
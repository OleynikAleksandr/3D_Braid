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

            var minDiamParam = new FollowingParam
            {
                Name = "Min Diameter",
                NickName = "mm",
                Description = "Минимальный диаметр сечения (мм)",
                Access = GH_ParamAccess.item
            };
            minDiamParam.SetPersistentData(1.4);
            pManager.AddParameter(minDiamParam);

            var maxDiamParam = new FollowingParam
            {
                Name = "Max Diameter",
                NickName = "mm",
                Description = "Максимальный диаметр сечения (мм)",
                Access = GH_ParamAccess.item
            };
            maxDiamParam.SetPersistentData(2.0);
            pManager.AddParameter(maxDiamParam);

            var rotAngleParam = new FollowingParam
            {
                Name = "Rotation Angle",
                NickName = "deg",
                Description = "Угол поворота сечений (градусы)",
                Access = GH_ParamAccess.item
            };
            rotAngleParam.SetPersistentData(56.0);
            pManager.AddParameter(rotAngleParam);
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
                    if (!_slidersCreated)
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
                CreateSlider(document, 0, "mm\u00A0", 6.0, BraidParameters.WIDTH_MIN, BraidParameters.WIDTH_MAX, sliderOffset, GH_SliderAccuracy.Float);
                CreateSlider(document, 1, "mm\u00A0", 2.0, BraidParameters.HEIGHT_MIN, BraidParameters.HEIGHT_MAX, sliderOffset, GH_SliderAccuracy.Float);
                CreateSlider(document, 2, "mm\u00A0", 18.0, BraidParameters.DIAMETER_MIN, BraidParameters.DIAMETER_MAX, sliderOffset, GH_SliderAccuracy.Float);
                CreateSlider(document, 3, "mm\u00A0", 0.2, BraidParameters.OFFSET_MIN, BraidParameters.OFFSET_MAX, sliderOffset, GH_SliderAccuracy.Float);
                CreateSlider(document, 4, "num", 14.0, BraidParameters.PERIODS_MIN, BraidParameters.PERIODS_MAX, sliderOffset, GH_SliderAccuracy.Integer);
                CreateSlider(document, 5, "mm\u00A0", 1.4, BraidParameters.MIN_DIAMETER_MIN, BraidParameters.MIN_DIAMETER_MAX, sliderOffset, GH_SliderAccuracy.Float);
                CreateSlider(document, 6, "mm\u00A0", 2.0, BraidParameters.MAX_DIAMETER_MIN, BraidParameters.MAX_DIAMETER_MAX, sliderOffset, GH_SliderAccuracy.Float);
                CreateSlider(document, 7, "deg\u00A0", 56.0, BraidParameters.ROTATION_ANGLE_MIN, BraidParameters.ROTATION_ANGLE_MAX, sliderOffset, GH_SliderAccuracy.Float);
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

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double width = 6.0;
            double height = 0.0;
            double diameter = 0.0;
            double diameterOffset = 0.0;
            double numPeriodsDouble = 0.0;
            double minDiameter = 0.0;
            double maxDiameter = 0.0;
            double rotationAngle = 0.0;

            if (!DA.GetData(0, ref width)) return;
            if (!DA.GetData(1, ref height)) return;
            if (!DA.GetData(2, ref diameter)) return;
            if (!DA.GetData(3, ref diameterOffset)) return;
            if (!DA.GetData(4, ref numPeriodsDouble)) return;
            if (!DA.GetData("Min Diameter", ref minDiameter)) return;
            if (!DA.GetData("Max Diameter", ref maxDiameter)) return;
            if (!DA.GetData("Rotation Angle", ref rotationAngle)) return;

            // Преобразуем в целые числа
            int numPeriods = (int)Math.Round(numPeriodsDouble);            

            _debugCircle = new Circle(new Plane(Point3d.Origin, Vector3d.ZAxis, Vector3d.XAxis), diameter / 2);

            try
            {
                _parameters.Width = width;
                _parameters.Height = height;
                _parameters.Diameter = diameter;
                _parameters.DiameterOffset = diameterOffset;
                _parameters.NumPeriods = numPeriods;
                _parameters.MinDiameter = minDiameter;
                _parameters.MaxDiameter = maxDiameter;
                _parameters.RotationAngle = rotationAngle;

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
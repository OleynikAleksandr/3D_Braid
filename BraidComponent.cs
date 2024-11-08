using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel.Special;
using Grasshopper.GUI.Base;
using Grasshopper;
using Grasshopper.Kernel.Types;

namespace _3D_Braid
{
    public class BraidComponent : GH_Component
    {
        private readonly BraidParameters _parameters;
        private readonly BraidGeometryGenerator _generator;
        private bool _sliderCreated = false;

        private struct SliderConfig
        {
            public string Name;
            public string NickName;
            public string Description;
            public double Min;
            public double Max;
            public double Default;
            public bool IsInteger;
            public int ParamIndex;
        }

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

            pManager.AddNumberParameter("Height", "Height", "Высота косички (мм)", GH_ParamAccess.item, 2.0);
            pManager.AddNumberParameter("Steepness", "Steepness", "Крутизна косички (мм)", GH_ParamAccess.item, 1.0);
            pManager.AddIntegerParameter("Points/Period", "Points/Period", "Точки на период", GH_ParamAccess.item, 20);
            pManager.AddNumberParameter("Diameter", "Diameter", "Диаметр кольца (мм)", GH_ParamAccess.item, 18.0);
            pManager.AddNumberParameter("Diameter Offset", "Diameter Offset", "Смещение диаметра (мм)", GH_ParamAccess.item, 0.2);
            pManager.AddIntegerParameter("Num Periods", "Num Periods", "Количество периодов", GH_ParamAccess.item, 14);
            pManager.AddCurveParameter("Section", "Section", "Секционная кривая", GH_ParamAccess.item);
        }

        public override void AddedToDocument(GH_Document document)
        {
            base.AddedToDocument(document);

            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Компонент добавлен в документ");

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += (sender, e) =>
            {
                if (!_sliderCreated && this.Attributes != null &&
                    Params.Input.All(p => p.Attributes != null))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Таймер сработал - начинаем создание элементов управления");
                    CreateControls(document);
                    _sliderCreated = true;
                    timer.Stop();
                    timer.Dispose();
                }
            };
            timer.Start();
        }

        private void CreateControls(GH_Document document)
        {
            try
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Начинаем создание слайдеров");

                var sliderConfigs = new[]
                {
                    new SliderConfig
                    {
                        Name = "Width",
                        NickName = "Width",
                        Description = "Ширина косички (мм)",
                        Min = BraidParameters.WIDTH_MIN,
                        Max = BraidParameters.WIDTH_MAX,
                        Default = 6.0,
                        IsInteger = false,
                        ParamIndex = 0
                    },
                    new SliderConfig
                    {
                        Name = "Height",
                        NickName = "Height",
                        Description = "Высота косички (мм)",
                        Min = BraidParameters.HEIGHT_MIN,
                        Max = BraidParameters.HEIGHT_MAX,
                        Default = 2.0,
                        IsInteger = false,
                        ParamIndex = 1
                    },
                    new SliderConfig
                    {
                        Name = "Steepness",
                        NickName = "Steepness",
                        Description = "Крутизна косички (мм)",
                        Min = BraidParameters.STEEPNESS_MIN,
                        Max = BraidParameters.STEEPNESS_MAX,
                        Default = 1.0,
                        IsInteger = false,
                        ParamIndex = 2
                    },
                    new SliderConfig
                    {
                        Name = "Points/Period",
                        NickName = "Points/Period",
                        Description = "Точки на период",
                        Min = BraidParameters.POINTS_MIN,
                        Max = BraidParameters.POINTS_MAX,
                        Default = 20,
                        IsInteger = true,
                        ParamIndex = 3
                    },
                    new SliderConfig
                    {
                        Name = "Diameter",
                        NickName = "Diameter",
                        Description = "Диаметр кольца (мм)",
                        Min = BraidParameters.DIAMETER_MIN,
                        Max = BraidParameters.DIAMETER_MAX,
                        Default = 18.0,
                        IsInteger = false,
                        ParamIndex = 4
                    },
                    new SliderConfig
                    {
                        Name = "Diameter Offset",
                        NickName = "Diameter Offset",
                        Description = "Смещение диаметра (мм)",
                        Min = BraidParameters.OFFSET_MIN,
                        Max = BraidParameters.OFFSET_MAX,
                        Default = 0.2,
                        IsInteger = false,
                        ParamIndex = 5
                    },
                    new SliderConfig
                    {
                        Name = "Num Periods",
                        NickName = "Num Periods",
                        Description = "Количество периодов",
                        Min = BraidParameters.PERIODS_MIN,
                        Max = BraidParameters.PERIODS_MAX,
                        Default = 14,
                        IsInteger = true,
                        ParamIndex = 6
                    }
                };

                var maxNameWidth = sliderConfigs.Max(c =>
                    GH_FontServer.StringWidth(c.NickName, GH_FontServer.Standard));

                float sliderWidth = maxNameWidth + 60;

                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Создаем слайдеры");

                foreach (var config in sliderConfigs)
                {
                    var param = Params.Input[config.ParamIndex] as IGH_Param;
                    if (param != null)
                    {
                        var slider = new GH_NumberSlider();
                        slider.CreateAttributes();

                        slider.Attributes.Bounds = new RectangleF(
                            slider.Attributes.Bounds.X,
                            slider.Attributes.Bounds.Y,
                            sliderWidth,
                            slider.Attributes.Bounds.Height
                        );

                        slider.Attributes.Pivot = new PointF(
                            (float)this.Attributes.DocObject.Attributes.Bounds.Left - sliderWidth - 30,
                            (float)param.Attributes.Bounds.Y
                        );

                        slider.Name = config.Name;
                        slider.NickName = config.NickName;
                        slider.Description = config.Description;

                        if (slider.Slider != null)
                        {
                            slider.Slider.Type = config.IsInteger ? GH_SliderAccuracy.Integer : GH_SliderAccuracy.Float;
                            slider.Slider.Minimum = Convert.ToDecimal(config.Min);
                            slider.Slider.Maximum = Convert.ToDecimal(config.Max);
                            slider.Slider.DecimalPlaces = config.IsInteger ? 0 : 1;
                            slider.Slider.Value = Convert.ToDecimal(config.Default);
                            slider.Slider.GripDisplay = GH_SliderGripDisplay.ShapeAndText;
                        }

                        document.AddObject(slider, false);
                        param.AddSource(slider);
                    }
                }

                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Слайдеры созданы, создаем компонент Curve");

                // Создаем компонент Curve
                try
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Создаем компонент Curve");

                    var curve = new Grasshopper.Kernel.Types.GH_Curve();
                    if (curve == null)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Не удалось создать GH_Curve");
                        return;
                    }

                    curve.CreateAttributes();

                    // Позиционируем под последним слайдером
                    var lastSliderParam = Params.Input[6];
                    float curveX = (float)this.Attributes.DocObject.Attributes.Bounds.Left - sliderWidth - 30;
                    float curveY = (float)lastSliderParam.Attributes.Bounds.Bottom + 20;

                    curve.Attributes.Pivot = new PointF(curveX, curveY);

                    // Добавляем на канвас
                    document.AddObject(curve, false);

                    // Подключаем к входу Section
                    var sectionParam = Params.Input[7];
                    if (curve.Params?.Output.Count > 0)
                    {
                        sectionParam.AddSource(curve.Params.Output[0]);
                    }

                    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Компонент Curve успешно добавлен");
                }
                catch (Exception ex)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                        "Ошибка при создании Curve: " + ex.Message + "\n" + ex.StackTrace);
                }

                document.NewSolution(true);
            }
            catch (Exception ex)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                    "Ошибка создания элементов управления: " + ex.Message);
            }
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Breps", "B", "Результирующая геометрия", GH_ParamAccess.tree);
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
    }
}
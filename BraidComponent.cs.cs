using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Drawing;

namespace _3D_Braid
{
    public class BraidComponent : GH_Component
    {
        private BraidParameters _parameters;
        private BraidGeometryGenerator _generator;

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
            pManager.AddNumberParameter("Width", "W", "Ширина косички (мм)", GH_ParamAccess.item, 6.0);
            pManager.AddNumberParameter("Height", "H", "Высота косички (мм)", GH_ParamAccess.item, 2.0);
            pManager.AddNumberParameter("Steepness", "S", "Крутизна косички (мм)", GH_ParamAccess.item, 1.0);
            pManager.AddIntegerParameter("Points/Period", "Pts", "Точки на период", GH_ParamAccess.item, 20);
            pManager.AddNumberParameter("Diameter", "D", "Диаметр кольца (мм)", GH_ParamAccess.item, 18.0);
            pManager.AddNumberParameter("Diameter Offset", "DO", "Смещение диаметра (мм)", GH_ParamAccess.item, 0.2);
            pManager.AddIntegerParameter("Num Periods", "N", "Количество периодов", GH_ParamAccess.item, 14);
            pManager.AddCurveParameter("Section", "Sec", "Секционная кривая", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Breps", "B", "Результирующая геометрия", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Получаем входные данные
            double width = 0.0;
            double height = 0.0;
            double steepness = 0.0;
            int pointsPeriod = 0;
            double diameter = 0.0;
            double diameterOffset = 0.0;
            int numPeriods = 0;
            Curve sectionCurve = null;

            // Получаем все параметры
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
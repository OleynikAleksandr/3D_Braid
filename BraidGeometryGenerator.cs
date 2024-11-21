using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper;

namespace _3D_Braid
{
    public class BraidGeometryGenerator
    {
        private readonly BraidParameters _parameters;
        private const int pointsPeriod = 20;

        public BraidGeometryGenerator(BraidParameters parameters)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }
        public GH_Structure<GH_Brep> GenerateBraid()
        {
            try
            {
                var polarArray = new GH_Structure<GH_Brep>();
                double calculatedDiameter = _parameters.Diameter + (2 * _parameters.DiameterOffset);
                double circumference = Math.PI * calculatedDiameter;
                double exactPeriodLength = circumference / _parameters.NumPeriods;
                double targetLength = 2 * Math.PI * (_parameters.MaxDiameter / 2);

                // Генерация основных точек траектории
                var pointsTree = GenerateSineWavePoints(_parameters.Width, exactPeriodLength, _parameters.Height,
                                                      calculatedDiameter, pointsPeriod, _parameters.NumPeriods);

                // Преобразование дерева точек в плоский список
                List<Point3d> outputPoints = new List<Point3d>();
                GH_Path path = new GH_Path(0);

                // Берем все точки включая хвостики для сглаживания
                for (int i = 0; i < pointsTree[path].Count; i++)
                {
                    outputPoints.Add(pointsTree[path][i].Value);
                }

                // Создание сглаженной базовой кривой через точки
                Curve railCurve = Curve.CreateInterpolatedCurve(outputPoints, 3);
                // Ограничиваем minDiameter
                double limitedMinDiameter = Math.Min(_parameters.MinDiameter, targetLength / Math.PI);
                double minRadius = limitedMinDiameter / 2;

                // Расчет большой полуоси эллипса по формуле Рамануджана
                double majorRadius = minRadius;
                double step = minRadius;
                double currentLength;

                do
                {
                    majorRadius += step;
                    double h = Math.Pow((majorRadius - minRadius) / (majorRadius + minRadius), 2);
                    currentLength = Math.PI * (majorRadius + minRadius) * (1 + 3 * h / (10 + Math.Sqrt(4 - 3 * h)));

                    if (Math.Abs(currentLength - targetLength) > 0.001)
                    {
                        if (currentLength > targetLength)
                        {
                            majorRadius -= step;
                            step /= 2;
                        }
                    }
                } while (Math.Abs(currentLength - targetLength) > 0.001 && step > 1e-6);
                // Создаем список для всех кривых сечений
                List<Curve> sectionCurves = new List<Curve>();

                // Инициализация списков для плоскостей в контрольных точках
                List<Plane> basePlanes = new List<Plane>();
                List<Plane> realPlanes = new List<Plane>();
                int[] controlPoints = new int[] { 5, 10, 15, 20, 25 };

                // Создание и модификация плоскостей в контрольных точках
                foreach (int index in controlPoints)
                {
                    if (index < outputPoints.Count)
                    {
                        Point3d point = outputPoints[index];
                        double parameter;
                        railCurve.ClosestPoint(point, out parameter);
                        Plane framePlane;
                        railCurve.FrameAt(parameter, out framePlane);
                        basePlanes.Add(framePlane);

                        // Создаем плоскость перпендикулярную касательному вектору
                        Plane realPlane = new Plane(
                            framePlane.Origin,      // та же точка
                            framePlane.YAxis,       // YAxis frame-плоскости = касательный вектор = новая нормаль
                            framePlane.ZAxis        // используем Z как ориентир
                        );

                        // Поворачиваем плоскости в точках 15 и 20 на 180 градусов
                        if (index == 15 || index == 20)
                        {
                            realPlane.Rotate(Math.PI, realPlane.Normal);
                        }

                        // Применяем общий поворот
                        if (index == 15)
                        {
                            realPlane.Rotate((-_parameters.RotationAngle * Math.PI / 180.0) - 195 * Math.PI / 180.0, realPlane.Normal);
                        }
                        else if (index == 5 || index == 25)
                        {
                            realPlane.Rotate((_parameters.RotationAngle * Math.PI / 180.0) + 195 * Math.PI / 180.0, realPlane.Normal);
                        }

                        realPlanes.Add(realPlane);
                    }
                }
                // Создаем окружность на Plane 10
                Circle circle10 = new Circle(realPlanes[1], _parameters.MaxDiameter / 2);
                Curve circleCurve10 = new ArcCurve(circle10);
                sectionCurves.Add(circleCurve10);

                // Переносим окружность на Plane 20 методом PlaneToPlane
                Transform xform = Transform.PlaneToPlane(realPlanes[1], realPlanes[3]);
                Circle circle20 = circle10;
                circle20.Transform(xform);
                sectionCurves.Add(new ArcCurve(circle20));

                // Создаем эллипсы на Plane 5, 15 и 25 с перемещением швов
                // Эллипс 5 - шов на малой оси (+90°)
                Ellipse ellipse5 = new Ellipse(realPlanes[0], majorRadius, minRadius);
                var curve5 = ellipse5.ToNurbsCurve();
                var splitCurves5 = curve5.Split(Math.PI / 2);
                if (splitCurves5 != null && splitCurves5.Length == 2)
                {
                    var newCurve5 = Curve.JoinCurves(new[] { splitCurves5[1], splitCurves5[0] });
                    if (newCurve5 != null && newCurve5.Length > 0)
                        sectionCurves.Add(newCurve5[0]);
                }

                // Эллипс 15 - шов на малой оси (+270°)
                Ellipse ellipse15 = new Ellipse(realPlanes[2], majorRadius, minRadius);
                var curve15 = ellipse15.ToNurbsCurve();
                var splitCurves15 = curve15.Split(3 * Math.PI / 2);
                if (splitCurves15 != null && splitCurves15.Length == 2)
                {
                    var newCurve15 = Curve.JoinCurves(new[] { splitCurves15[1], splitCurves15[0] });
                    if (newCurve15 != null && newCurve15.Length > 0)
                        sectionCurves.Add(newCurve15[0]);
                }

                // Эллипс 25 - шов на малой оси (+90°)
                Ellipse ellipse25 = new Ellipse(realPlanes[4], majorRadius, minRadius);
                var curve25 = ellipse25.ToNurbsCurve();
                var splitCurves25 = curve25.Split(Math.PI / 2);
                if (splitCurves25 != null && splitCurves25.Length == 2)
                {
                    var newCurve25 = Curve.JoinCurves(new[] { splitCurves25[1], splitCurves25[0] });
                    if (newCurve25 != null && newCurve25.Length > 0)
                        sectionCurves.Add(newCurve25[0]);
                }
                // Создаем поверхность
                Brep[] sweep = Brep.CreateFromSweep(railCurve, sectionCurves, true, 0.01);
                if (sweep == null || sweep.Length == 0)
                    return polarArray;

                List<Brep> allBreps = new List<Brep>();
                double angleStep = 2.0 * Math.PI / _parameters.NumPeriods;

                // Добавляем первый период
                allBreps.Add(sweep[0].DuplicateBrep());

                // Создание остальных периодов через поворот
                for (int i = 1; i < _parameters.NumPeriods; i++)
                {
                    Transform rotation = Transform.Rotation(i * angleStep, Vector3d.YAxis, Point3d.Origin);
                    Brep transformedBrep = sweep[0].DuplicateBrep();
                    transformedBrep.Transform(rotation);
                    allBreps.Add(transformedBrep);
                }

                // Создание единой пряди через объединение
                Brep joinedBrep = sweep[0].DuplicateBrep();
                for (int i = 1; i < _parameters.NumPeriods; i++)
                {
                    Transform rotation = Transform.Rotation(i * angleStep, Vector3d.YAxis, Point3d.Origin);
                    Brep transformedBrep = sweep[0].DuplicateBrep();
                    transformedBrep.Transform(rotation);
                    joinedBrep.Join(transformedBrep, 0.01, true);
                }
                // Создаем три пряди с разным смещением
                double offsetAngle = angleStep / 3.0;

                // Добавляем центральную прядь
                polarArray.Append(new GH_Brep(joinedBrep), new GH_Path(0));

                // Создаем прядь смещенную по часовой
                Brep clockwiseStrand = joinedBrep.DuplicateBrep();
                clockwiseStrand.Transform(Transform.Rotation(offsetAngle, Vector3d.YAxis, Point3d.Origin));
                polarArray.Append(new GH_Brep(clockwiseStrand), new GH_Path(1));

                // Создаем прядь смещенную против часовой
                Brep counterClockwiseStrand = joinedBrep.DuplicateBrep();
                counterClockwiseStrand.Transform(Transform.Rotation(-offsetAngle, Vector3d.YAxis, Point3d.Origin));
                polarArray.Append(new GH_Brep(counterClockwiseStrand), new GH_Path(2));

                return polarArray;
            }
            catch (Exception)
            {
                return new GH_Structure<GH_Brep>();
            }
        }

        // Здесь будут вспомогательные методы
        private GH_Structure<GH_Point> GenerateSineWavePoints(double width, double exactPeriodLength, double height, double calculatedDiameter, int pointsPerPeriod, int numPeriods)
        {
            var pointsTree = new GH_Structure<GH_Point>();
            GH_Path path = new GH_Path(0);

            // Генерируем точки с дополнительными полупериодами в начале и конце
            int totalPoints = pointsPerPeriod + 10; // Добавляем по 5 точек с каждой стороны
            double tIncrement = exactPeriodLength / pointsPerPeriod;
            double circumference = Math.PI * calculatedDiameter;

            for (int i = -5; i <= pointsPerPeriod + 5; i++)
            {
                double t = i * tIncrement;
                double angle = (t / circumference) * (2 * Math.PI);

                double phaseAdjustedT = (2 * Math.PI * i) / pointsPerPeriod;
                double offsetY = width / 2 * Math.Sin(phaseAdjustedT);
                double offsetR = height / 2 * Math.Sin(2 * phaseAdjustedT);

                double x = (calculatedDiameter / 2 + offsetR) * Math.Cos(angle);
                double y = offsetY;
                double z = (calculatedDiameter / 2 + offsetR) * Math.Sin(angle);

                Point3d point = new Point3d(x, y, z);
                pointsTree.Append(new GH_Point(point), path);
            }

            return pointsTree;
        }
    }
}
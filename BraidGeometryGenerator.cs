using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace _3D_Braid
{
    public class BraidGeometryGenerator
    {
        private readonly BraidParameters _parameters;

        public BraidGeometryGenerator(BraidParameters parameters)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        public GH_Structure<GH_Brep> GenerateBraid()
        {
            // Создание сечения по умолчанию, если нет входного сечения
            if (_parameters.SectionCurve == null)
            {
                _parameters.SectionCurve = new Ellipse(Plane.WorldXY, 0.5, 0.8).ToNurbsCurve();
            }

            var pointsTree = GenerateSineWavePoints();
            var railCurve = CreateInterpolatedCurve(pointsTree);

            Curve[] sectionCurves = CreateSectionCurvesAtStartAndEnd(railCurve);
            var surface = CreateSurfaceAroundCurve(railCurve, sectionCurves);

            if (surface != null)
            {
                Brep closedSurface = CreateLoftSurfaceBetweenSections(surface, sectionCurves[0], sectionCurves[1]);

                if (closedSurface != null)
                {
                    return CreateThreeSurfaceArray(closedSurface);
                }
            }

            return null;
        }

        private GH_Structure<GH_Point> GenerateSineWavePoints()
        {
            double amplitudeFactor = NormalizeAmplitude(_parameters.Steepness);
            double frequency = (2 * _parameters.NumPeriods) / _parameters.Diameter;
            double periodLength = 2 * Math.PI / frequency;
            double circumference = Math.PI * _parameters.Diameter;

            int numPeriods = (int)(circumference / periodLength);
            double adjustedPeriodLength = circumference / numPeriods;
            double tIncrement = adjustedPeriodLength / _parameters.PointsPeriod;

            var pointsTree = new GH_Structure<GH_Point>();
            GH_Path path = new GH_Path(0);
            Point3d firstPoint = Point3d.Unset;

            for (int period = 0; period < numPeriods; period++)
            {
                for (int i = 0; i < _parameters.PointsPeriod; i++)
                {
                    double t = period * adjustedPeriodLength + i * tIncrement;
                    double angle = (t / circumference) * (2 * Math.PI);

                    double phaseAdjustedT = (2 * Math.PI * i) / _parameters.PointsPeriod;
                    double offsetY = (i == 0) ? 0 : (_parameters.Width / 2) * Math.Sin(phaseAdjustedT);
                    double offsetR = (_parameters.Height / 2) * amplitudeFactor *
                                   Math.Atan(_parameters.Steepness * Math.Sin(2 * phaseAdjustedT)) / (Math.PI / 2);

                    double x = (_parameters.Diameter / 2 + offsetR) * Math.Cos(angle);
                    double y = offsetY;
                    double z = (_parameters.Diameter / 2 + offsetR) * Math.Sin(angle);

                    Point3d point = new Point3d(x, y, z);
                    if (period == 0 && i == 0)
                    {
                        firstPoint = point;
                    }
                    pointsTree.Append(new GH_Point(point), path);
                }
            }

            if (firstPoint.IsValid)
            {
                pointsTree[path].Add(new GH_Point(firstPoint));
            }

            return pointsTree;
        }

        private Curve CreateInterpolatedCurve(GH_Structure<GH_Point> pointsTree)
        {
            List<Point3d> points = new List<Point3d>();
            foreach (GH_Point ghPoint in pointsTree.AllData(true))
            {
                points.Add(ghPoint.Value);
            }
            return Curve.CreateInterpolatedCurve(points, 3);
        }

        private Curve[] CreateSectionCurvesAtStartAndEnd(Curve railCurve)
        {
            double closeToEndOffset = 0.00001;

            railCurve.PerpendicularFrameAt(railCurve.Domain.Min, out Plane startPlane);
            Curve startSection = _parameters.SectionCurve.DuplicateCurve();
            startSection.Transform(Transform.PlaneToPlane(Plane.WorldXY, startPlane));

            double endParam = railCurve.Domain.Min + ((railCurve.Domain.Max - railCurve.Domain.Min) * (1.0 - closeToEndOffset));
            Point3d nearEndPoint = railCurve.PointAt(endParam);
            Transform moveToNearEnd = Transform.Translation(nearEndPoint - startPlane.Origin);

            Curve endSection = startSection.DuplicateCurve();
            endSection.Transform(moveToNearEnd);

            return new Curve[] { startSection, endSection };
        }

        private Brep CreateSurfaceAroundCurve(Curve railCurve, Curve[] sectionCurves)
        {
            Brep[] sweep = Brep.CreateFromSweep(railCurve, sectionCurves, false, 0.1);
            return (sweep != null && sweep.Length > 0) ? sweep[0] : null;
        }

        private Brep CreateLoftSurfaceBetweenSections(Brep surface, Curve startSection, Curve endSection)
        {
            List<Curve> loftCurves = new List<Curve> { startSection, endSection };
            Brep[] loftSurfaces = Brep.CreateFromLoft(loftCurves, Point3d.Unset, Point3d.Unset, LoftType.Tight, false);

            if (loftSurfaces.Length > 0)
            {
                List<Brep> surfacesToJoin = new List<Brep> { surface, loftSurfaces[0] };
                Brep[] joinedSurfaces = Brep.JoinBreps(surfacesToJoin, 0.001);

                if (joinedSurfaces.Length > 0)
                {
                    return joinedSurfaces[0];
                }
            }

            return null;
        }

        private GH_Structure<GH_Brep> CreateThreeSurfaceArray(Brep baseSurface)
        {
            var polarArray = new GH_Structure<GH_Brep>();
            double frequency = (2 * _parameters.NumPeriods) / _parameters.Diameter;
            double periodLength = 2 * Math.PI / frequency;
            double angleOffset = periodLength / (3 * (_parameters.Diameter / 2));

            Brep originalSurface = baseSurface.DuplicateBrep();
            polarArray.Append(new GH_Brep(originalSurface), new GH_Path(0));

            Brep rotatedSurfaceClockwise = baseSurface.DuplicateBrep();
            rotatedSurfaceClockwise.Transform(Transform.Rotation(angleOffset, Vector3d.YAxis, Point3d.Origin));
            polarArray.Append(new GH_Brep(rotatedSurfaceClockwise), new GH_Path(1));

            Brep rotatedSurfaceCounterClockwise = baseSurface.DuplicateBrep();
            rotatedSurfaceCounterClockwise.Transform(Transform.Rotation(-angleOffset, Vector3d.YAxis, Point3d.Origin));
            polarArray.Append(new GH_Brep(rotatedSurfaceCounterClockwise), new GH_Path(2));

            return polarArray;
        }

        private double NormalizeAmplitude(double steepness)
        {
            return (Math.PI / 2) / Math.Atan(steepness);
        }
    }
}

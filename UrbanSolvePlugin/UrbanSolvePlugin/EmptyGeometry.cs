using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace UrbanSolvePlugin
{
    class EmptyGeometry : BuildingGeometry
    {
        public EmptyGeometry(double length, double width, double depth, int numberOfFloors,
            double storeyHeight, double rotation, bool isCentered, Point3d position)
        {
            geometry = new Brep();

            List<Brep> facesList = new List<Brep>();
            List<Brep> walls = new List<Brep>();
            Brep roof = null;

            // Create a plane surface
            Interval intervalX = new Interval(-length / 2, length / 2);
            Interval intervalY = new Interval(-width / 2, width / 2);
            PlaneSurface planeSrf = new PlaneSurface(Plane.WorldXY, intervalX, intervalY);
            Point3d buildingCenter = Point3d.Origin;

            Brep floor = Brep.CreateFromSurface(planeSrf);
            PlaneSurface srfToSubstractTypo2 = new PlaneSurface(Plane.WorldXY, new Interval(intervalX.T0 + depth, intervalX.T1 - depth),
                new Interval(intervalY.T0 + depth, intervalY.T1 - depth));
            Brep floorToSubstractTypo2 = Brep.CreateFromSurface(srfToSubstractTypo2);
            Brep[] floorListTypo2 = Brep.CreateBooleanDifference(floor, floorToSubstractTypo2, 0.001);

            try
            {
                // If the result of the difference is more than one surface, something went wrong
                if (floorListTypo2.Length != 1)
                {
                    throw new Exception();
                }
                floor = floorListTypo2[0];
                facesList.Add(floor);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                throw;
            }

            // Get the perimeter of the floor
            if (floor != null)
            {
                Curve[] perimeter = floor.DuplicateEdgeCurves();
                Vector3d translationVect = new Vector3d(Plane.WorldXY.OriginX, Plane.WorldXY.OriginY, numberOfFloors * storeyHeight);
                foreach (Curve curve in perimeter)
                {
                    Surface srf = Surface.CreateExtrusion(curve, translationVect);
                    Brep face = srf.ToBrep();
                    facesList.Add(face);
                }

                roof = floor.DuplicateBrep();
                roof.Translate(translationVect);
                facesList.Add(roof);
            }
            else
            {
                geometry = null;
            }

            // Creates volume
            Brep[] solidList = null;
            try
            {
                solidList = Brep.CreateSolid(facesList, 0.001);
                if (solidList.Length != 1)
                {
                    geometry = null;
                }
                else
                {
                    geometry = solidList[0];
                }
            }
            catch (Exception e)
            {
                Rhino.RhinoApp.WriteLine(e.ToString());
            }

            // Verify volume
            if (!geometry.IsValid)
            {
                geometry = null;
                return;
            }

            // Rotate volume according to rotation
            geometry.Transform(Transform.Rotation(rotation, buildingCenter));

            // Translate volume
            if (isCentered)
            {
                geometry.Translate(new Vector3d(position) - new Vector3d(buildingCenter));
            }
            else
            {
                Vector3d v = new Vector3d(new Point3d(buildingCenter.X - (length / 2), buildingCenter.Y + (width / 2), buildingCenter.Z));
                v.Transform(Transform.Rotation(rotation, buildingCenter));
                geometry.Translate(new Vector3d(position) - v);
            }
        }

        public EmptyGeometry(EmptyBuilding baseBuilding)
        {
            geometry = new Brep();

            List<Brep> facesList = new List<Brep>();
            List<Brep> walls = new List<Brep>();
            Brep floor = null;
            Brep roof = null;

            // Create a plane surface
            Interval intervalX = new Interval(-baseBuilding.length / 2, baseBuilding.length / 2);
            Interval intervalY = new Interval(-baseBuilding.width / 2, baseBuilding.width / 2);
            PlaneSurface planeSrf = new PlaneSurface(Plane.WorldXY, intervalX, intervalY);
            Point3d buildingCenter = Point3d.Origin;

            floor = Brep.CreateFromSurface(planeSrf);
            PlaneSurface srfToSubstractTypo2 = new PlaneSurface(Plane.WorldXY, new Interval(intervalX.T0 + baseBuilding.depth, intervalX.T1 - baseBuilding.depth),
                new Interval(intervalY.T0 + baseBuilding.depth, intervalY.T1 - baseBuilding.depth));
            Brep floorToSubstractTypo2 = Brep.CreateFromSurface(srfToSubstractTypo2);
            Brep[] floorListTypo2 = Brep.CreateBooleanDifference(floor, floorToSubstractTypo2, 0.001);

            try
            {
                // If the result of the difference is more than one surface, something went wrong
                if (floorListTypo2.Length != 1)
                {
                    geometry = null;
                    return;
                }
                floor = floorListTypo2[0];
                facesList.Add(floor);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                throw;
            }

            // Get the perimeter of the floor
            if (floor != null)
            {
                Curve[] perimeter = floor.DuplicateEdgeCurves();
                Vector3d translationVect = new Vector3d(Plane.WorldXY.OriginX, Plane.WorldXY.OriginY, baseBuilding.numberOfFloors * baseBuilding.storeyHeight);
                foreach (Curve curve in perimeter)
                {
                    Surface srf = Surface.CreateExtrusion(curve, translationVect);
                    Brep face = srf.ToBrep();
                    facesList.Add(face);
                }

                roof = floor.DuplicateBrep();
                roof.Translate(translationVect);
                facesList.Add(roof);
            }
            else
            {
                geometry = null;
            }

            // Creates volume
            Brep[] solidList = null;

            try
            {
                solidList = Brep.CreateSolid(facesList, 0.001);
                if (solidList.Length != 1)
                {
                    geometry = null;
                }
                else
                {
                    geometry = solidList[0];
                }
            }
            catch (Exception e)
            {
                Rhino.RhinoApp.WriteLine(e.ToString());
                Rhino.RhinoApp.WriteLine("Error during volume creation");
            }

            // Verify volume
            if (!geometry.IsValid)
            {
                geometry = null;
                return;
            }

            // Rotate volume according to rotation
            geometry.Transform(Transform.Rotation(baseBuilding.rotation, buildingCenter));

            // Translate volume
            if (baseBuilding.isCentered)
            {
                geometry.Translate(new Vector3d(baseBuilding.refPoint) - new Vector3d(buildingCenter));
            }
            else
            {
                Vector3d v = new Vector3d(new Point3d(buildingCenter.X - (baseBuilding.length / 2), buildingCenter.Y + (baseBuilding.width / 2), buildingCenter.Z));
                v.Transform(Transform.Rotation(baseBuilding.rotation, buildingCenter));
                geometry.Translate(new Vector3d(baseBuilding.refPoint) - v);
            }
        }
    }
}





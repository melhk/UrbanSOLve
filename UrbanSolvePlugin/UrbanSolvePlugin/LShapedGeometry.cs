using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace UrbanSolvePlugin
{
    class LShapedGeometry : BuildingGeometry
    {
        public LShapedGeometry(double length, double width, double depth, int numberOfFloors, double storeyHeight,
            double rotation, bool isCentered, Point3d position)
        {
            geometry = new Brep();

            List<Brep> facesList = new List<Brep>();
            List<Brep> walls = new List<Brep>();

            Interval intervalX = new Interval(-length / 2.0, length / 2.0);
            Interval intervalY = new Interval(-width / 2.0, width / 2.0);
            PlaneSurface planeSrf = new PlaneSurface(Plane.WorldXY, intervalX, intervalY);
            Point3d buildingCenter = Point3d.Origin;

            Brep floor = Brep.CreateFromSurface(planeSrf);
            PlaneSurface srfToSubstractTypo3 = new PlaneSurface(Plane.WorldXY, new Interval(intervalX.T0 + depth, intervalX.T1),
                new Interval(intervalY.T0, intervalY.T1 - depth));
            Brep floorToSubstractTypo3 = Brep.CreateFromSurface(srfToSubstractTypo3);
            Brep[] floorListTypo3 = Brep.CreateBooleanDifference(floor, floorToSubstractTypo3, 0.001);
            try
            {
                // If the result of the difference is more than one surface, something went wrong
                if (floorListTypo3.Length != 1)
                {
                    geometry = null;
                    return;
                }
                floor = floorListTypo3[0];
                facesList.Add(floor);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }

            // Get the perimeter of the floor
            if (floor != null)
            {
                Curve[] perimeter = floor.DuplicateEdgeCurves();
                Vector3d translationVect = new Vector3d(Plane.WorldXY.OriginX, Plane.WorldXY.OriginY, storeyHeight * numberOfFloors);
                foreach (Curve curve in perimeter)
                {
                    Surface srf = Surface.CreateExtrusion(curve, translationVect);
                    Brep face = srf.ToBrep();
                    facesList.Add(face);
                }

                Brep roof = floor.DuplicateBrep();
                roof.Translate(translationVect);
                facesList.Add(roof);
            }
            else
            {
                geometry = null;
                return;
            }

            // Creates volume
            Brep[] solidList = null;

            try
            {
                solidList = Brep.CreateSolid(facesList, 0.001);
                if (solidList.Length != 1)
                {
                    geometry = null;
                    return;
                }
                else
                {
                    geometry = solidList[0];
                }
            }
            catch (Exception e)
            {
                Rhino.RhinoApp.WriteLine(e.ToString());
                throw;
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
                Vector3d v1 = new Vector3d(new Point3d(buildingCenter.X - (length / 2.0), buildingCenter.Y + (width / 2.0), buildingCenter.Z));
                v1.Transform(Transform.Rotation(rotation, buildingCenter));
                geometry.Translate(new Vector3d(position) - v1);
            }
        }

        public LShapedGeometry(LShapedBuilding baseBuilding)
        {
            geometry = new Brep();

            List<Brep> facesList = new List<Brep>();
            Brep floor = null;
            List<Brep> walls = new List<Brep>();

            Interval intervalX = new Interval(-baseBuilding.length / 2.0, baseBuilding.length / 2.0);
            Interval intervalY = new Interval(-baseBuilding.width / 2.0, baseBuilding.width / 2.0);
            PlaneSurface planeSrf = new PlaneSurface(Plane.WorldXY, intervalX, intervalY);
            Point3d buildingCenter = Point3d.Origin;

            floor = Brep.CreateFromSurface(planeSrf);
            PlaneSurface srfToSubstractTypo3 = new PlaneSurface(Plane.WorldXY, new Interval(intervalX.T0 + baseBuilding.depth, intervalX.T1),
                new Interval(intervalY.T0, intervalY.T1 - baseBuilding.depth));
            Brep floorToSubstractTypo3 = Brep.CreateFromSurface(srfToSubstractTypo3);
            Brep[] floorListTypo3 = Brep.CreateBooleanDifference(floor, floorToSubstractTypo3, 0.001);
            try
            {
                // If the result of the difference is more than one surface, something went wrong
                if (floorListTypo3.Length != 1)
                {
                    geometry = null;
                    return;
                }
                floor = floorListTypo3[0];
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
                Vector3d translationVect = new Vector3d(Plane.WorldXY.OriginX, Plane.WorldXY.OriginY, baseBuilding.storeyHeight * baseBuilding.numberOfFloors);
                foreach (Curve curve in perimeter)
                {
                    Surface srf = Surface.CreateExtrusion(curve, translationVect);
                    Brep face = srf.ToBrep();
                    facesList.Add(face);
                }

                Brep roof = floor.DuplicateBrep();
                roof.Translate(translationVect);
                facesList.Add(roof);
            }
            else
            {
                geometry = null;
                return;
            }

            // Creates volume
            Brep[] solidList = null;

            try
            {
                solidList = Brep.CreateSolid(facesList, 0.001);
                if (solidList.Length != 1)
                {
                    geometry = null;
                    return;
                }
                else
                {
                    geometry = solidList[0];
                }
            }
            catch (Exception e)
            {
                Rhino.RhinoApp.WriteLine(e.ToString());
                throw;
            }
          
            // Verify volume
            if (!geometry.IsValid)
            {
                geometry = null;
                return;
            }

            // Rotate volume according to rotation
            Transform rotation = Transform.Rotation(baseBuilding.rotation, buildingCenter);
            geometry.Transform(rotation);

            // Translate volume
            if (baseBuilding.isCentered)
            {
                geometry.Translate(new Vector3d(baseBuilding.refPoint) - new Vector3d(buildingCenter));
            }
            else
            {        
                Vector3d v1 = new Vector3d(new Point3d(buildingCenter.X - (baseBuilding.length / 2.0), buildingCenter.Y + (baseBuilding.width / 2.0), buildingCenter.Z));
                v1.Transform(rotation);
                geometry.Translate(new Vector3d(baseBuilding.refPoint) - v1);             
            }
        }
    }
}

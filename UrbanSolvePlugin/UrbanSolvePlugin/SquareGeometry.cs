using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace UrbanSolvePlugin
{
    class SquareGeometry : BuildingGeometry
    {
        public SquareGeometry(double length, double width, int numberOfFloors, double storeyHeight,
            double rotation, bool isCentered, Point3d position)
        {
            geometry = new Brep();

            List<Brep> facesList = new List<Brep>();    // List of Breps to join in a volume
            List<Brep> walls = new List<Brep>();
            Brep floor = null;
            Brep roof = null;

            Interval intervalX = new Interval(-length / 2, length / 2);
            Interval intervalY = new Interval(-width / 2, width / 2);
            PlaneSurface planeSrf = new PlaneSurface(Plane.WorldXY, intervalX, intervalY);
            Point3d buildingCenter = Point3d.Origin;

            floor = Brep.CreateFromSurface(planeSrf);
            facesList.Add(floor);

            // Get the perimeter of the floor
            if (floor != null)
            {
                Curve[] perimeter = floor.DuplicateEdgeCurves();
                Vector3d translationVect = new Vector3d(Plane.WorldXY.OriginX, Plane.WorldXY.OriginY, numberOfFloors * storeyHeight);
                foreach (Curve curve in perimeter)
                {
                    // Extrude each curve and add the new surface to the list of breps to join
                    Surface srf = Surface.CreateExtrusion(curve, translationVect);
                    // Surface to Brep
                    Brep face = srf.ToBrep();
                    // Add to the list of breps to join
                    facesList.Add(face);
                }

                // Creates roof
                roof = floor.DuplicateBrep();
                roof.Translate(translationVect);
                facesList.Add(roof);
            }
            else
            {
                throw new Exception();
            }

            // Creates volume
            Brep[] solidList = null;

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

            // Verify volume
            if (!geometry.IsValid)
            {
                throw new Exception();
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

        public SquareGeometry(SquareBuilding baseBuilding)
        {
            geometry = new Brep();

            List<Brep> facesList = new List<Brep>();    // List of Breps to join in a volume
            List<Brep> walls = new List<Brep>();
            Brep floor = null;
            Brep roof = null;

            Interval intervalX = new Interval(-baseBuilding.length / 2, baseBuilding.length / 2);
            Interval intervalY = new Interval(-baseBuilding.width / 2, baseBuilding.width / 2);
            PlaneSurface planeSrf = new PlaneSurface(Plane.WorldXY, intervalX, intervalY);
            Point3d buildingCenter = Point3d.Origin;

            floor = Brep.CreateFromSurface(planeSrf);
            facesList.Add(floor);

            // Get the perimeter of the floor
            if (floor != null)
            {
                Curve[] perimeter = floor.DuplicateEdgeCurves();
                Vector3d translationVect = new Vector3d(Plane.WorldXY.OriginX, Plane.WorldXY.OriginY, baseBuilding.numberOfFloors * baseBuilding.storeyHeight);
                foreach (Curve curve in perimeter)
                {
                    // Extrude each curve and add the new surface to the list of breps to join
                    Surface srf = Surface.CreateExtrusion(curve, translationVect);
                    // Surface to Brep
                    Brep face = srf.ToBrep();
                    // Add to the list of breps to join
                    facesList.Add(face);
                }

                // Creates roof
                roof = floor.DuplicateBrep();
                roof.Translate(translationVect);
                facesList.Add(roof);
            }
            else
            {
                throw new Exception();
            }

            // Creates volume
            Brep[] solidList = null;

            solidList = Brep.CreateSolid(facesList, 0.001);
            if (solidList.Length != 1)
            {
                throw new Exception();
            }
            else
            {
                geometry = solidList[0];
            }

            // Verify volume
            if (!geometry.IsValid)
            {
                throw new Exception();
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
 
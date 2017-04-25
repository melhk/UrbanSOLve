using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Rhino;
using System.Threading;
using Rhino.Geometry.Collections;
using System.Linq;

namespace UrbanSolvePlugin
{
    public class Variant
    {
        private static int counter = 0;
        public int Id { get; set; }

        public bool isValid { get; set; }

        public VariantDescription variantDescription { get; private set; }

        public List<Building> buildings { get; protected set; }
        public int totalNumberOfBuildings { get; protected set; }

        public int generation { get; private set; }
        public int solutionNumber { get; private set; }
        public int seed { get; private set; }
        public double minDistBtwBuildings { get; private set; }
        public double siteCoverage { get; private set; }
        public double floorAreaRatio { get; private set; }

        public double daylightAutonomy { get; private set; }
        public double activeSolarEnergy { get; private set; }
        public double energyNeed { get; private set; }

        public double totalFloorArea { get; private set; }
        public double totalEnvelopArea { get; private set; }
        public double totalGroundArea { get; private set; }
        
        public Variant(int generation, int solutionNumber, VariantDescription variantDescription, int seed, double minDistBtwBuildings,
            double activeSolarEnergy, double daylightAutonomy, double energyNeed, Variant baseSolution,
            double totalFloorArea, double totalEnvelopArea, double totalGroundArea)
        {
            Id = Interlocked.Increment(ref counter);
            this.generation = generation;
            this.solutionNumber = solutionNumber;
            this.variantDescription = variantDescription;
            buildings = baseSolution.buildings;
            this.seed = seed;
            this.minDistBtwBuildings = minDistBtwBuildings;
            this.activeSolarEnergy = activeSolarEnergy;
            this.daylightAutonomy = daylightAutonomy;
            this.energyNeed = energyNeed;
            this.totalFloorArea = totalFloorArea;
            this.totalEnvelopArea = totalEnvelopArea;
            this.totalGroundArea = totalGroundArea;

            totalNumberOfBuildings = variantDescription.buildingDescriptions.Count();
            siteCoverage = getSiteCoverage();
            floorAreaRatio = getFloorAreaRatio();
            isValid = true;
        }

        public Variant(int generation, int solutionNumber, VariantDescription variantDescription, int seed, double minDistBtwBuildings,
            double totalGroundArea)
        {
            Id = Interlocked.Increment(ref counter);
            this.variantDescription = variantDescription;
            buildings = new List<Building>();
            this.generation = generation;
            this.solutionNumber = solutionNumber;
            this.seed = seed;
            this.minDistBtwBuildings = minDistBtwBuildings;
            this.totalGroundArea = totalGroundArea;
            this.activeSolarEnergy = 0.0;
            this.daylightAutonomy = 0.0;
            this.energyNeed = 0.0;

            totalNumberOfBuildings = variantDescription.buildingDescriptions.Count();
            siteCoverage = getSiteCoverage();
            floorAreaRatio = getFloorAreaRatio();
            isValid = false;
        }

        public override string ToString()
        {
            string descr = "Generation " + generation + " - Variant " + (solutionNumber + 1) + Environment.NewLine;
            descr += "Is valid: " + isValid + Environment.NewLine;
            descr += "Seed: " + seed + Environment.NewLine;
            descr += "Minimum distance between buildings: " + minDistBtwBuildings + Environment.NewLine;
            descr += "Site coverage: " + siteCoverage + Environment.NewLine;
            descr += "Floor area ratio: " + floorAreaRatio + Environment.NewLine;
            descr += "Daylight autonomy: " + daylightAutonomy + Environment.NewLine;
            descr += "Active solar energy: " + activeSolarEnergy + Environment.NewLine;
            descr += "Energy need: " + energyNeed + Environment.NewLine + Environment.NewLine;

            int i = 1;
            foreach (Building b in buildings)
            {
                descr += "------ Building " + i.ToString() + " ------" + Environment.NewLine;
                descr += b.ToString() + Environment.NewLine + Environment.NewLine;
                ++i;
            }
            return descr;
        }

        public string toCsv()
        {
            string sep = "; ";
            string descr = generation + sep + (solutionNumber + 1) + sep + siteCoverage + sep + floorAreaRatio + sep
                + daylightAutonomy + sep + activeSolarEnergy + sep + energyNeed + sep;

            for (int i = 0; i < buildings.Count; ++i)
            {
                if (i < buildings.Count - 1)
                {
                    descr += buildings.ElementAt(i).ToCsv() + sep;
                }
                else
                {
                    descr += buildings.ElementAt(i).ToCsv() + ";";
                }
            }
            return descr;
        }

        public List<Brep> getGeometry()
        {
            List<Brep> breps = new List<Brep>();
            foreach(Building building in buildings)
            {
                breps.Add(building.geometry.geometry);
            }
            return breps;
        } 

        public void createPoints()
        {       
            foreach (Building building in buildings)
            {
                building.irradiationPoints.Clear();
                BrepFaceList faces = building.geometry.geometry.Faces;

                for (int i = 0; i < faces.Count; ++i)
                {
                    Interval uDim = faces[i].Domain(0);
                    Interval vDim = faces[i].Domain(1);

                    int uNbOfPoints = (int)Math.Floor(uDim.Length);
                    int vNbOfPoints = (int)Math.Floor(vDim.Length);
                    double uRem = Math.Floor(uDim.Length);
                    double vRem = Math.Floor(vDim.Length);

                    int uStart = (int)Math.Ceiling(uDim.Min);
                    int uEnd = (int)Math.Floor(uDim.Max);
                    int vStart = (int)Math.Ceiling(vDim.Min);
                    int vEnd = (int)Math.Floor(vDim.Max);

                    // Centrer la grille de points sur la surface
                    if (uRem != 0.0)
                    {
                        uRem = (uDim.Length % Math.Floor(uDim.Length)) / 2;
                    }
                    else
                    {
                        uNbOfPoints -= 1;
                        uRem = 0.5;
                    }
                    if (vRem != 0.0)
                    {
                        vRem = (vDim.Length % Math.Floor(vDim.Length)) / 2;
                    }
                    else
                    {
                        vNbOfPoints -= 1;
                        vRem = 0.5;
                    }

                    for (int u = uStart; u <= uEnd; ++u)
                    {
                        for (int v = vStart; v <= vEnd; ++v)
                        {
                            if (faces[i].IsPointOnFace(u + uRem, v + vRem) == PointFaceRelation.Interior)
                            {
                                Point3d point = faces[i].PointAt(u + uRem, v + vRem);
                                Vector3d normal = faces[i].NormalAt(u + uRem, v + vRem);

                                // les points sont légèrement décalés de la surface
                                point += normal * 0.01;

                                if (!(normal.Z < 0 && normal.X == 0 && normal.Y == 0))
                                {
                                    IrradiationPoint irradiationPoint = new IrradiationPoint(point, normal);
                                    building.irradiationPoints.Add(irradiationPoint);
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool checkMinimumDistance()
        {
            Curve[] intersectionCurves = null;
            Point3d[] intersectionPoints = null;

            if (buildings.Count > 1)
            {
                try
                {
                    for (int i = 0; i <= buildings.Count - 2; ++i)
                    {
                        Brep b1 = buildings[i].geometry.geometry;

                        for (int j = i + 1; j <= buildings.Count - 1; ++j)
                        {
                            Brep b2 = buildings[j].geometry.geometry;
                            bool success = true;

                            // Check overlapping
                            success = Rhino.Geometry.Intersect.Intersection.BrepBrep(b1, b2, 0.001, out intersectionCurves, out intersectionPoints);
                            if ((intersectionCurves.Length != 0 || intersectionPoints.Length != 0) && success)
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    RhinoApp.WriteLine(e.ToString());
                }
                Array.Clear(intersectionCurves, 0, intersectionCurves.Length);
                Array.Clear(intersectionPoints, 0, intersectionPoints.Length);
            }
            return true;
        }

        public bool checkOnParcel()
        {
            Curve[] intersectionCurves = null;
            Point3d[] intersectionPoints = null;

            try
            {
                foreach (Building building in buildings)
                {
                    intersectionCurves = null;
                    intersectionPoints = null;

                    foreach (Brep parcel in variantDescription.ground)
                    {
                        Curve[] contours = parcel.DuplicateEdgeCurves();
                        foreach (Curve curve in contours)
                        {
                            bool success = Rhino.Geometry.Intersect.Intersection.CurveBrep(curve, building.geometry.geometry, 0.001, 
                                out intersectionCurves, out intersectionPoints);
                            if ((intersectionCurves.Length != 0 || intersectionPoints.Length != 0) && success)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                RhinoApp.WriteLine(e.ToString());
            }
            return true;
        }

        public double getFacadeIrradiationPerTotalFloorArea()
        {
            double totalFloorArea = 0.0;
            double totalFacadeIrradiation = 0.0;

            foreach (Building building in buildings)
            {
                totalFloorArea += building.getTotalFloorArea();
                for (int i = 0; i < 4; ++i)
                {
                    totalFacadeIrradiation += building.getIrradiation().ElementAt(i);
                }
            }
            try
            {
                return totalFacadeIrradiation / totalFloorArea;
            }
            catch (DivideByZeroException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }   
        }

        public double getAverageFacadeIrradiation()
        {
            double averageFacadeIrradiation = 0.0;

            foreach (Building building in buildings)
            {
                averageFacadeIrradiation += building.getMeanIrradiation().ElementAt(5);
            }
  
            try
            {
                return averageFacadeIrradiation / buildings.Count;
            }
            catch (DivideByZeroException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }
        }

        public double getAverageEnvelopIrradiation()
        {
            double averageEnvelopIrradiation = 0.0;

            foreach (Building building in buildings)
            {
                averageEnvelopIrradiation += building.getMeanIrradiation().ElementAt(5);
            }

            try
            {
                return averageEnvelopIrradiation / buildings.Count;
            }
            catch (DivideByZeroException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }
        }

        public List<double> getAverageIrradiationPerOrientation()
        {
            double[] averageIrradiationPerOrientation = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0 };

            foreach (Building building in buildings)
            {
                double[] meanBuildingIrradiation = building.getMeanIrradiation();
                averageIrradiationPerOrientation[0] += meanBuildingIrradiation.ElementAt(0);
                averageIrradiationPerOrientation[1] += meanBuildingIrradiation.ElementAt(1);
                averageIrradiationPerOrientation[2] += meanBuildingIrradiation.ElementAt(2);
                averageIrradiationPerOrientation[3] += meanBuildingIrradiation.ElementAt(3);
                averageIrradiationPerOrientation[4] += meanBuildingIrradiation.ElementAt(4);
            }

            for (int i = 0; i < averageIrradiationPerOrientation.Length; ++i)
            {
                averageIrradiationPerOrientation[i] /= buildings.Count;
            }

            return averageIrradiationPerOrientation.ToList<double>();
        }

        public double getSiteCoverage()
        {
            try
            {
                return getFootprintArea() / variantDescription.totalGroundArea;
            }
            catch (DivideByZeroException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return 0.0;
            }
        }

        public double getFloorAreaRatio()
        {
            try
            {
                return getTotalFloorArea() / variantDescription.totalGroundArea;
            } 
            catch (DivideByZeroException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return 0.0;
            }
        }

        public bool checkSiteCoverage()
        {
            double siteCoverage = getSiteCoverage();
            if (siteCoverage < variantDescription.minSiteCoverage | siteCoverage > variantDescription.maxSiteCoverage)
            {
                return false;
            }
            return true;
        }

        public bool checkFloorAreaRatio()
        {
            double floorAreaRatio = getFloorAreaRatio();
            if (floorAreaRatio < variantDescription.minFloorAreaRatio | floorAreaRatio > variantDescription.maxFloorAreaRatio)
            {
                return false;
            }
            return true;
        }

        public double getFootprintArea()
        {
            return buildings.Sum(building => building.getFootprintArea());
        }

        public double getTotalFloorArea()
        {
            return buildings.Sum(building => building.getTotalFloorArea());
        }

        public double getTotalRoofArea()
        {
            return buildings.Sum(building => building.getRoofArea());
        }

        public double getTotalEnvelopArea()
        {
            return buildings.Sum(building => building.getTotalEnvelopeArea());
        }

        public void addBuilding(Building building)
        {
            buildings.Add(building);
            totalNumberOfBuildings = buildings.Count;
        }

        public double getTotalWindowArea()
        {
            return buildings.Sum(building => building.getTotalFloorArea() * building.description.glazingRatio);
        }
    }
}
 
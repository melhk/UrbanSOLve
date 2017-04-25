using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;

namespace UrbanSolvePlugin
{

    public class VariantDescription
    {
        public int seed { get; set; }
        public int numberOfGenerations { get; set; }
        public int populationSize { get; set; }
        public double minDistanceBetweenBuildings { get; set; }

        public double minSiteCoverage { get; set; }
        public double maxSiteCoverage { get; set; }

        public double minFloorAreaRatio { get; set; }
        public double maxFloorAreaRatio { get; set; }

        public List<Brep> context { get; private set; }
        public List<Brep> ground { get; private set; }
        public double totalGroundArea { get; private set; }

        private BoundingBox b;
        public Plane p { get; private set; }
        public Rectangle3d r { get; private set; }

        public List<BuildingDescription> buildingDescriptions { get; private set; }
    
        public Variant initialVariant { get; set; }
        public List<List<Variant>> simulationResults { get; private set; }

        public VariantDescription()
        {
            numberOfGenerations = CST.NB_OF_GENERATIONS;
            populationSize = CST.POPULATION_SIZE;

            minSiteCoverage = CST.MIN_SITE_COVERAGE;
            maxSiteCoverage = CST.MAX_SITE_COVERAGE;

            minFloorAreaRatio = CST.MIN_FLOOR_AREA_RATIO;
            maxFloorAreaRatio = CST.MAX_FLOOR_AREA_RATIO;

            context = new List<Brep>();
            ground = new List<Brep>();
            totalGroundArea = 0.0;
            buildingDescriptions = new List<BuildingDescription>();
            initialVariant = null;
            simulationResults = new List<List<Variant>>();
        }

        public override string ToString()
        {
            string text = "";
            text += "Number of generations: " + numberOfGenerations + Environment.NewLine;
            text += "Number of variants per generation: " + populationSize + Environment.NewLine;
            text += "Seed: " + seed + Environment.NewLine;
            text += String.Format("Ground area: {0} m2" + Environment.NewLine, (double)decimal.Round((decimal)totalGroundArea, 2, MidpointRounding.AwayFromZero));
            text += "Climate: Geneva" + Environment.NewLine;
            text += String.Format("Site coverage: {0} (min {1}, max {2})" + Environment.NewLine, (double)decimal.Round((decimal)initialVariant.getSiteCoverage(), 2, MidpointRounding.AwayFromZero), minSiteCoverage, maxSiteCoverage);
            text += String.Format("Floor area ratio: {0} (min {1}, max {2})" + Environment.NewLine + Environment.NewLine, (double)decimal.Round((decimal)initialVariant.getFloorAreaRatio(), 2, MidpointRounding.AwayFromZero), minFloorAreaRatio, maxFloorAreaRatio);

            int i = 1;
            foreach (BuildingDescription b in buildingDescriptions)
            {
                text += String.Format("------ Building {0} ------" + Environment.NewLine, i.ToString());
                text += b.ToString() + Environment.NewLine + Environment.NewLine;
                ++i;
            }
            return text;
        }

        public string toCsv()
        {
            string text = "Generation; Variant; Site coverage; Floor area ratio; Daylit area [%]; Electricity production [kWh/m2]; Energy need [kWh/m2]; Building dimensions (L-W-(D)-H)[m];" + Environment.NewLine;
            text += initialVariant.toCsv() + Environment.NewLine;
            foreach (List<Variant> generation in simulationResults)
            {
                foreach (Variant variant in generation)
                {
                    text += variant.toCsv() + Environment.NewLine;
                }
            }
            return text;
        }

        public void setContext(List<Brep> breps)
        {
            context.Clear();
            context.AddRange(breps);
        }

        public void setGround(List<Brep> breps)
        {
            ground.Clear();
            ground.AddRange(breps);

            totalGroundArea = breps.Sum(b => b.GetArea());

            b = breps.ElementAt(0).GetBoundingBox(false);
            foreach (Brep brep in breps)
            {
                b.Union(brep.GetBoundingBox(false));
            }
            p = new Plane(b.GetCorners()[0], b.GetCorners()[1], b.GetCorners()[2]);
            r = new Rectangle3d(p, b.Min, b.Max);
        }

        public double getMinimumPossibleSiteCoverage()
        {
            double footprintArea = 0.0;
            foreach (BuildingDescription b in buildingDescriptions)
            {
                Building building = b.getBuilding(b.getLowerBounds());
                footprintArea += building.getFootprintArea();
            }
            return footprintArea / totalGroundArea;
        }

        public double getMaximumPossibleSiteCoverage()
        {
            double footprintArea = 0.0;
            foreach (BuildingDescription b in buildingDescriptions)
            {
                Building building = b.getBuilding(b.getUpperBounds());
                footprintArea += building.getFootprintArea();
            }
            return footprintArea / totalGroundArea;
        }

        public double getMinimumPossibleFloorAreaRatio()
        {
            double floorArea = 0.0;
            foreach (BuildingDescription b in buildingDescriptions)
            {
                Building building = b.getBuilding(b.getLowerBounds());
                floorArea += building.getTotalFloorArea();
            }
            return floorArea / totalGroundArea;
        }

        public double getMaximumPossibleFloorAreaRatio()
        {
            double floorArea = 0.0;
            foreach (BuildingDescription b in buildingDescriptions)
            {
                Building building = b.getBuilding(b.getUpperBounds());
                floorArea += building.getTotalFloorArea();
            }
            return floorArea / totalGroundArea;
        }

        public int getNumberOfVariables()
        {
            return buildingDescriptions.Sum(building => building.getNumberOfVariables());
        }

        public void addBuildingDescription(BuildingDescription description)
        {
            buildingDescriptions.Add(description);
        }

        public Variant getVariant(int generation, int variantNumber)
        {
            try
            {
                return simulationResults.ElementAt(generation).ElementAt(variantNumber);
            }
            catch (ArgumentOutOfRangeException e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                throw;
            }
        }

        public void clearVariants()
        {
            simulationResults.Clear();
            initialVariant = null;
        }

        public void clearBuildingDescriptions()
        {
            buildingDescriptions.Clear();
        }

        public Variant getSolution(int generation, int solutionNumber)
        {
            try
            {
                return simulationResults.ElementAt(generation).ElementAt(solutionNumber);
            }
            catch (IndexOutOfRangeException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }
        }

        public void saveGeneration(List<Variant> s)
        {
            simulationResults.Add(s);
        }

        public Variant getInitialSolution()
        {
            Variant solution = new Variant(0, 0, this, seed, minDistanceBetweenBuildings, totalGroundArea);
            List<Building> buildings = new List<Building>();

            foreach (BuildingDescription descr in buildingDescriptions)
            {
                buildings.Add(descr.getStartBuilding());
            }

            if (buildingDescriptions.Count() == buildings.Count())
            {
                foreach (Building b in buildings)
                {
                    solution.addBuilding(b);
                }
                return solution;
            }
            return null;
        }

        public Variant getSolution(int generationNumber, int variantNumber, double[] values)
        {
            Variant solution = new Variant(generationNumber, variantNumber, this, this.seed, this.minDistanceBetweenBuildings, 
                this.totalGroundArea);
            // Vérifie la géométrie
            if (checkValues(values))
            {
                foreach (Building building in getBuildings(values))
                {
                    solution.addBuilding(building);
                }
            }
            return solution;
        }

        public double[] getInitialSolutionValues()
        {
            List<double> initialValues = new List<double>();
            foreach (BuildingDescription descr in buildingDescriptions)
            {
                initialValues.AddRange(descr.getStartBuilding().description.getActualValues());
            }
            return initialValues.ToArray();
        }

        public List<Building> getBuildings(double[] values)
        {
            List<Building> buildings = new List<Building>();
            Type buildingType;
            int numberOfParameters;
            int index = 0;

            foreach (BuildingDescription buildingDescription in buildingDescriptions)
            {
                buildingType = buildingDescription.GetType();
                numberOfParameters = buildingDescription.getNumberOfVariables();

                if (buildingType == typeof(SquareDescription))
                {
                    var segment = new ArraySegment<double>(values, index, numberOfParameters);
                    try
                    {
                        double length = segment.ElementAt(0);
                        double width = segment.ElementAt(1);
                        double storeyNumber = segment.ElementAt(2);
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.ToString());
                        throw;
                    }               
                    buildings.Add(buildingDescription.getBuilding(segment.ToArray<double>()));
                }
                else if (buildingType == typeof(EmptyDescription))
                {
                    var segment = new ArraySegment<double>(values, index, numberOfParameters);
                    double length = segment.ElementAt(0);
                    double width = segment.ElementAt(1);
                    double storeyNumber = segment.ElementAt(2);
                    double depth = segment.ElementAt(3);
                    buildings.Add(buildingDescription.getBuilding(segment.ToArray<double>()));
                }
                else if (buildingType == typeof(LShapedDescription))
                {
                    var segment = new ArraySegment<double>(values, index, numberOfParameters);
                    double length = segment.ElementAt(0);
                    double width = segment.ElementAt(1);
                    double storeyNumber = segment.ElementAt(2);
                    double depth = segment.ElementAt(3);
                    buildings.Add(buildingDescription.getBuilding(segment.ToArray<double>()));
                }
                index += numberOfParameters;
            }
            return buildings;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns>true si les valeurs permettent de générer la liste de bâtiments, faux sinon.</returns>
        public bool checkValues(double[] values)
        {
            Type buildingType;
            int numberOfParameters;
            int index = 0;
            
            // Pour chaque bâtiment de la liste de bâtiments à générer
            foreach (BuildingDescription descr in buildingDescriptions)
            {
                buildingType = descr.GetType();
                numberOfParameters = descr.getNumberOfVariables();

                if (buildingType == typeof(SquareDescription))
                {
                   // continue;
                }
                else if (buildingType == typeof(EmptyDescription))
                {
                    var segment = new ArraySegment<double>(values, index, numberOfParameters);
                    try
                    {
                        double length = segment.ElementAt(0);
                        double width = segment.ElementAt(1);
                        double storeyNumber = segment.ElementAt(2);
                        double depth = segment.ElementAt(3);
                        if (2 * depth >= length - CST.MIN_CENTER_SIZE || 2 * depth >= width - CST.MIN_CENTER_SIZE)
                        {
                            return false;
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.ToString());
                        throw;
                    }
                }
                else if (buildingType == typeof(LShapedDescription))
                {
                    var segment = new ArraySegment<double>(values, index, numberOfParameters);
                    try
                    {
                        double length = segment.ElementAt(0);
                        double width = segment.ElementAt(1);
                        double storeyNumber = segment.ElementAt(2);
                        double depth = segment.ElementAt(3);

                        if (depth >= length || depth >= width)
                        {
                            return false;
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.ToString());
                        throw;
                    }
                }
                index += numberOfParameters;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] getLowerBounds()
        {
            List<double> lowerBounds = new List<double>();
            foreach (BuildingDescription buildingDescription in buildingDescriptions)
            {
                foreach (double bound in buildingDescription.getLowerBounds())
                {
                    lowerBounds.Add(bound);
                }
            } 
            return lowerBounds.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] getUpperBounds()
        {
            List<double> upperBounds = new List<double>();
            foreach (BuildingDescription buildingDescription in buildingDescriptions)
            {
                foreach (double bound in buildingDescription.getUpperBounds())
                {
                    upperBounds.Add(bound);
                }
            }
            return upperBounds.ToArray();
        }
    }
}

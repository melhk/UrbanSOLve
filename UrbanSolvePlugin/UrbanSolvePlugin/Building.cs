using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UrbanSolvePlugin
{
    /// <summary>
    /// Classe de base pour un bâtiment appartenant à une variante possible.
    /// </summary>
    public abstract class Building
    {
        // Le bâtiment correspond à la description souhaitée par l'utilisateur
        public virtual BuildingDescription description { get; private set; }
        // Position choisie par l'utilisateur, centre ou bord du bâtiment
        public Point3d refPoint { get; private set; }
        public double length { get; private set; }
        public double width { get; private set; }
        public double storeyHeight { get; private set; }
        public int numberOfFloors { get; private set; }
        // Rotation in radians
        public double rotation { get; private set; }
        // The building grows from a corner or its center
        public bool isCentered { get; private set; }
        // Geometry associated to this building
        public BuildingGeometry geometry { get; protected set; }

        public double energyNeed { get; private set; }
        public double activeSolarEnergy { get; private set; }
        public double daylightAutonomy { get; protected set; }
        
        public List<IrradiationPoint> irradiationPoints { get; set; }

        public Building(BuildingDescription description, Point3d refPoint, double length,
            double width, int numberOfFloors,
            double storeyHeight, double rotation, bool isCentered)
        {
            this.refPoint = refPoint;
            this.description = description;
            this.length = length;
            this.width = width;
            this.numberOfFloors = numberOfFloors;
            this.storeyHeight = storeyHeight;
            this.rotation = rotation;
            this.isCentered = isCentered;
            geometry = null;
            irradiationPoints = new List<IrradiationPoint>();
        }

        // Daylit area
        public abstract double getDaylightAutonomy(double floorAreaRatio, double siteCoverage); 
        /*
        {
            double buildingPerformance = 0.0;
            switch (description.program)
            {
                case Program.office:
                    buildingPerformance = CST.da_off_constant
                        + CST.da_off_plotRatio * floorAreaRatio
                        + CST.da_off_siteCoverage * siteCoverage
                        + CST.da_off_exEnvArea * getTotalEnvelopeArea()
                        + CST.da_off_footprintArea * getFootprintArea()
                        + CST.da_off_formFactor * getFormFactor()
                        + CST.da_off_eastFacRatio * getEastFacRatio()
                        + CST.da_off_southFacRatio * getSouthFacRatio()
                        + CST.da_off_wwRatio * description.glazingRatio
                        + CST.da_off_wfRatio * getWfRatio()
                        + CST.da_off_meanRoofIrr * getMeanIrradiation().ElementAt(4)
                        + CST.da_off_meanFacIrr * getMeanIrradiation().ElementAt(6)
                        + CST.da_off_meanSouthFacIrr * getMeanIrradiation().ElementAt(1)
                        + CST.da_off_facIrrPerFa * irrPerFloorArea().ElementAt(6)
                        + CST.da_off_eastFacIrrPrFa * irrPerFloorArea().ElementAt(2)

                        + CST.da_off_PlotRatio_ExpEnvArea * floorAreaRatio * getTotalEnvelopeArea()
                        + CST.da_off_SiteCoverage_MeanFacIrrad * siteCoverage * getMeanIrradiation().ElementAt(6)
                        + CST.da_off_ExpEnvArea_FootprintArea * getTotalEnvelopeArea() * getFootprintArea()
                        + CST.da_off_ExpEnvArea_FormFactor * getTotalEnvelopeArea() * getFormFactor()
                        + CST.da_off_FootprintArea_MeanRoofIrrad * getFootprintArea() * getMeanIrradiation().ElementAt(4)
                        + CST.da_off_FormFactor_FacIrradPerFA * getFormFactor() * irrPerFloorArea().ElementAt(6)
                        + CST.da_off_EastFacRatio_MeanFacIrrad * getEastFacRatio() * getMeanIrradiation().ElementAt(6)
                        + CST.da_off_WWRatio_MeanFacIrrad * description.glazingRatio * getMeanIrradiation().ElementAt(6)
                        + CST.da_off_WFRatio_FacIrradPerFA * getWfRatio() * irrPerFloorArea().ElementAt(6);
                    break;
                case Program.residential:
                    buildingPerformance = CST.da_app_constant
                        + CST.da_app_expEnvArea * getTotalEnvelopeArea()
                        + CST.da_app_formFactor * getFormFactor()
                        + CST.da_app_wwRatio * description.glazingRatio
                        + CST.da_app_wfRatio * getWfRatio()
                        + CST.da_app_meanRoofIrr * getMeanIrradiation().ElementAt(4)
                        + CST.da_app_meanFacIrr * getMeanIrradiation().ElementAt(6)
                        + CST.da_app_roofIrrPerFa * irrPerFloorArea().ElementAt(4)
                        + CST.da_app_northFacIrrPerFa * irrPerFloorArea().ElementAt(0)
                        + CST.da_app_southFacIrrPerFa * irrPerFloorArea().ElementAt(1)

                        + CST.da_app_ExpEnvArea_WFRatio * getTotalEnvelopeArea() * getWfRatio()
                        + CST.da_app_FormFactor_MeanRoofIrrad * getFormFactor() * getMeanIrradiation().ElementAt(4)
                        + CST.da_app_WWRatio_WFRatio * description.glazingRatio * getWfRatio()
                        + CST.da_app_WFRatio_MeanFacIrrad * getWfRatio() * getMeanIrradiation().ElementAt(6)
                        + CST.da_app_WFRatio_RoofIrradPerFA * getWfRatio() * irrPerFloorArea().ElementAt(4);
                    break;
            }

            if (buildingPerformance < 0)
            {
                buildingPerformance = 0;
            }
            else if (buildingPerformance > 100)
            {
                buildingPerformance = 100;
            }

            daylightAutonomy = buildingPerformance;
            return buildingPerformance;
        }
        */

        public double getActiveSolarEnergy()
        {
            List<IrradiationPoint> roofSensors = getRoofSensors();
            List<IrradiationPoint> facadeSensors = getFacadeSensors();
            double roofProduction = 0.17 * ((getRoofArea() / roofSensors.Count) * roofSensors.Sum(sensor => sensor.irradiationValue));
            double facadeProduction = (1 - description.glazingRatio) * 0.17 * ((getTotalFacadeArea() / facadeSensors.Count) * facadeSensors.Sum(sensor => sensor.irradiationValue));
            activeSolarEnergy = roofProduction + facadeProduction / getFootprintArea();
            return roofProduction + facadeProduction;
        }

        public double getEnergyNeed(double floorAreaRatio, double siteCoverage)
        {
            double buildingPerformance = 0.0;
            switch (description.program)
            {
                case Program.office:
                    buildingPerformance = CST.off_constant
                        + CST.off_plotRatio * floorAreaRatio
                        + CST.off_siteCoverage * siteCoverage
                        + CST.off_formFactor * getFormFactor()
                        + CST.off_wwRatio * description.glazingRatio
                        + CST.off_wfRatio * getWfRatio()
                        + CST.off_meanRoofIrr * getMeanIrradiation().ElementAt(4)
                        + CST.off_meanSouthFacIrr * getMeanIrradiation().ElementAt(1)
                        + CST.off_envelopIrrPerFa * irrPerFloorArea().ElementAt(5)
                        + CST.off_meanEnvelopIrr * getMeanIrradiation().ElementAt(5)
                        + CST.off_roofIrrPerFa * irrPerFloorArea().ElementAt(4)
                        + CST.off_northFacIrrPerFa * irrPerFloorArea().ElementAt(0)
                        + CST.off_southFacIrrPerFa * irrPerFloorArea().ElementAt(1);
                    break;
                case Program.residential:
                    buildingPerformance = CST.app_constant
                        + CST.app_nbOfFloors * numberOfFloors
                        + CST.app_formFactor * getFormFactor()
                        + CST.app_northFacRatio * getNorthFacRatio()
                        + CST.app_wwRatio * description.glazingRatio
                        + CST.app_wfRatio * getWfRatio()
                        + CST.app_meanNorthFacIrr * getMeanIrradiation().ElementAt(0)
                        + CST.app_meanSouthFacIrr * getMeanIrradiation().ElementAt(1)
                        + CST.app_eastFacIrrPerFa * irrPerFloorArea().ElementAt(2)
                        + CST.app_roofRatio * getRoofRatio()
                        + CST.app_meanEnvelopIrr * getMeanIrradiation().ElementAt(5);
                    break;
            }
            energyNeed = buildingPerformance;
            return buildingPerformance;
        }

        /// <summary>
        /// Surface de plancher totale
        /// </summary>
        /// <returns>
        /// total floor area [m^2]
        /// </returns>
        public double getTotalFloorArea()
        {
            return numberOfFloors * getFootprintArea();
        }

        /// <summary>
        /// Surface de plancher totale / surface de l'enveloppe totale
        /// </summary>
        /// <returns>
        /// form factor
        /// </returns>
        public double getFormFactor()
        {
            return getTotalFloorArea() / getTotalFacadeArea();
        }

        /// <summary>
        /// Surface d'enveloppe totale
        /// </summary>
        /// <returns>
        /// total envelop area [m^2]
        /// </returns>
        public double getTotalEnvelopeArea()
        {
            return getTotalFacadeArea() + getRoofArea();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// Roof area [m^2]
        /// </returns>
        public double getRoofArea()
        {
            return getFootprintArea();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double[] getIrradiation()
        {
            // North, South, East, West, Top, Total, facades
            double[] irradiation = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

            foreach (IrradiationPoint point in irradiationPoints)
            {
                Vector3d vector = point.vector;

                // Find the corresponding plane
                // North
                if (vector.X >= 0 && vector.Y > 0 && vector.Z < 0.5)
                {
                    irradiation[0] += point.irradiationValue;
                    irradiation[5] += point.irradiationValue;
                    irradiation[6] += point.irradiationValue;
                    continue;
                }

                // South
                if (vector.X <= 0 && vector.Y < 0 && vector.Z < 0.5)
                {
                    irradiation[1] += point.irradiationValue;
                    irradiation[5] += point.irradiationValue;
                    irradiation[6] += point.irradiationValue;
                    continue;
                }

                // East
                if (vector.X > 0 && vector.Y <= 0 && vector.Z < 0.5)
                {
                    irradiation[2] += point.irradiationValue;
                    irradiation[5] += point.irradiationValue;
                    irradiation[6] += point.irradiationValue;
                    continue;
                }

                // West
                if (vector.X < 0 && vector.Y >= 0 && vector.Z < 0.5)
                {
                    irradiation[3] += point.irradiationValue;
                    irradiation[5] += point.irradiationValue;
                    irradiation[6] += point.irradiationValue;
                    continue;
                }

                // Top
                if (vector.Z >= 0.5)
                {
                    irradiation[4] += point.irradiationValue;
                    irradiation[5] += point.irradiationValue;
                    continue;
                }
            }
            return irradiation;
        }

        private List<IrradiationPoint>[] getSensors()
        {
            List<IrradiationPoint> roofIrradiation = new List<IrradiationPoint>();
            List<IrradiationPoint> facadeIrradiation = new List<IrradiationPoint>();
            foreach (IrradiationPoint point in irradiationPoints)
            {
                if (point.vector.Z >= 0.5)
                {
                    roofIrradiation.Add(point);
                }
                else
                {
                    facadeIrradiation.Add(point);
                }
            }  
            return new List<IrradiationPoint>[] { facadeIrradiation, roofIrradiation };
        }

        public List<IrradiationPoint> getRoofSensors()
        {
            List<IrradiationPoint> sensors = getSensors().ElementAt(1);         
            return sensors.Where(irradiationPoint => irradiationPoint.irradiationValue >= 500).ToList();
        }

        public List<IrradiationPoint> getFacadeSensors()
        {
            List<IrradiationPoint> sensors = getSensors().ElementAt(0);
            return sensors.Where(irradiationPoint => irradiationPoint.irradiationValue >= 500).ToList();
        }

        public double getRoofRatio()
        {
            return getRoofArea() / getTotalEnvelopeArea();
        }

        public double[] getMeanIrradiation()
        {
            // North, South, East, West, Top, mean envelop, mean facades
            double[] meanIrradiation = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            int[] pointNumber = new int[] { 0, 0, 0, 0, 0, 0, 0 };

            foreach(IrradiationPoint point in irradiationPoints)
            {
                Vector3d vector = point.vector;

                // Find the corresponding plane
                // North
                if (vector.X >= 0 && vector.Y > 0 && vector.Z < 0.5)
                {
                    meanIrradiation[0] += point.irradiationValue;
                    pointNumber[0]++;
                    meanIrradiation[5] += point.irradiationValue;
                    pointNumber[5]++;
                    meanIrradiation[6] += point.irradiationValue;
                    pointNumber[6]++;
                    continue;
                }

                // South
                if (vector.X <= 0 && vector.Y < 0 && vector.Z < 0.5)
                {
                    meanIrradiation[1] += point.irradiationValue;
                    pointNumber[1]++;
                    meanIrradiation[5] += point.irradiationValue;
                    pointNumber[5]++;
                    meanIrradiation[6] += point.irradiationValue;
                    pointNumber[6]++;
                    continue;
                }

                // East
                if (vector.X > 0 && vector.Y <= 0 && vector.Z < 0.5)
                {
                    meanIrradiation[2] += point.irradiationValue;
                    pointNumber[2]++;
                    meanIrradiation[5] += point.irradiationValue;
                    pointNumber[5]++;
                    meanIrradiation[6] += point.irradiationValue;
                    pointNumber[6]++;
                    continue;
                }

                // West
                if (vector.X < 0 && vector.Y >= 0 && vector.Z < 0.5)
                {
                    meanIrradiation[3] += point.irradiationValue;
                    pointNumber[3]++;
                    meanIrradiation[5] += point.irradiationValue;
                    pointNumber[5]++;
                    meanIrradiation[6] += point.irradiationValue;
                    pointNumber[6]++;
                    continue;
                }

                // Top
                if (vector.Z >= 0.5)
                {
                    meanIrradiation[4] += point.irradiationValue;
                    pointNumber[4]++;
                    meanIrradiation[5] += point.irradiationValue;
                    pointNumber[5]++;
                    continue;
                }
            }

            for (int i = 0; i < meanIrradiation.Length; ++i)
            {
                meanIrradiation[i] /= pointNumber[i];
            }
            return meanIrradiation;
        }

        public double getWfRatio()
        {
            return description.glazingRatio / getTotalFloorArea();
        }

        // Rapport entre la surface de facade Nord et celle de l'enveloppe totale
        public double getNorthFacRatio()
        {
            return getFacadeArea().ElementAt(0) / getTotalEnvelopeArea();
        }

        public double getEastFacRatio()
        {
            return getFacadeArea().ElementAt(2) / getTotalEnvelopeArea();
        }

        public double getSouthFacRatio()
        {
            return getFacadeArea().ElementAt(1) / getTotalEnvelopeArea();
        }

        public double getWestFacRatio()
        {
            return getFacadeArea().ElementAt(3) / getTotalEnvelopeArea();
        }

        // Irradiation normalisée par surface de plancher
        // North, South, East, West, Top, Envelope, facades
        public double[] irrPerFloorArea()
        {
            double[] irradiation = getIrradiation();
            for (int i = 0; i < irradiation.Length; ++i)
            {
                irradiation[i] /= getTotalFloorArea();
            }
            return irradiation;
        }

        public double[] getFacadeArea()
        {
            double[] areas = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0 };
            try
            {
                List<Brep> facades = geometry.getFacades();
                foreach (Brep facade in facades)
                {
                    Surface surface = facade.Surfaces[0];
                    Vector3d vector = surface.NormalAt(0.5, 0.5);

                    // Find the corresponding plane
                    // North
                    if (vector.X >= 0 && vector.Y > 0 && vector.Z < 0.5)
                    {
                        areas[0] += facade.GetArea();
                        continue;
                    }

                    // South
                    if (vector.X <= 0 && vector.Y < 0 && vector.Z < 0.5)
                    {
                        areas[1] += facade.GetArea();
                        continue;
                    }

                    // East
                    if (vector.X > 0 && vector.Y <= 0 && vector.Z < 0.5)
                    {
                        areas[2] += facade.GetArea();
                        continue;
                    }

                    // West
                    if (vector.X < 0 && vector.Y >= 0 && vector.Z < 0.5)
                    {
                        areas[3] += facade.GetArea();
                        continue;
                    }

                    // Top
                    if (vector.Z >= 0.5)
                    {
                        areas[4] += facade.GetArea();
                        continue;
                    }
                }              
            }
            catch (NullReferenceException e) 
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            return areas;
        }

        public abstract double getFootprintArea();

        public abstract double getTotalFacadeArea();

        public abstract string ToCsv();
    }
}

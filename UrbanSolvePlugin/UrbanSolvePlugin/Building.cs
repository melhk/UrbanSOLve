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

        public double getActiveSolarEnergy()
        {
            List<IrradiationPoint> roofSensors = getRoofSensors();
            List<IrradiationPoint> facadeSensors = getFacadeSensors();
            double roofProduction = 0.17 * ((getRoofArea() / roofSensors.Count) * roofSensors.Sum(sensor => sensor.irradiationValue));
            double facadeProduction = (1 - description.glazingRatio) * 0.17 * ((getTotalFacadeArea() / facadeSensors.Count) * facadeSensors.Sum(sensor => sensor.irradiationValue));
            activeSolarEnergy = (roofProduction + facadeProduction) / getTotalFloorArea();
            return roofProduction + facadeProduction;
        }

        public double getEnergyNeed(double floorAreaRatio, double siteCoverage)
        {
            double buildingPerformance = 0.0;
            switch (description.program)
            {
                case Program.office:
                    buildingPerformance = 32.2978018230063
                        + 18.9284668032393 * getFormFactor()
                        + -3.42095697920017 * description.glazingRatio
                        + -2.84751491537644 * getWfRatio()
                        + -0.0317452282044676 * getMeanIrradiation().ElementAt(6) // MeanFacIrrad
                        + -0.000576084353749911 * getMeanIrradiation().ElementAt(0) // MeanNorthFacIrrad
                        + 0.000523589268194919 * getMeanIrradiation().ElementAt(1) // MeanSouthFacIrrad
                        + -0.0226333857127084 * getMeanIrradiation().ElementAt(3) // MeanWestFacIrrad
                        + -0.0109206283265959 * irrPerFloorArea().ElementAt(1) // SouthFacIrradPerFA
                        + -0.00724773184901299 * getFormFactor() * getMeanIrradiation().ElementAt(1)
                        + 0.0534332403142441 * getWfRatio() * getMeanIrradiation().ElementAt(6)
                        + 0.00917774739839259 * getWfRatio() * getMeanIrradiation().ElementAt(0)
                        + 0.0000412735619418759 * getMeanIrradiation().ElementAt(6) * getMeanIrradiation().ElementAt(3);
                    break;
                case Program.residential:
                    buildingPerformance = 19.1224187664128
                        + 22.668060572601 * getFormFactor()
                        + 29.0472635680661 * getWfRatio()
                        + -0.00238859308846743 * getMeanIrradiation().ElementAt(1) // MeanSouthFacIrrad
                        + 0.0604572394433875 * irrPerFloorArea().ElementAt(0) // NorthFacIrradPerFA
                        + 0.0173617724103075 * irrPerFloorArea().ElementAt(2) // EastFacIrradPerFA
                        + -0.0259159826090739 * irrPerFloorArea().ElementAt(1) // SouthFacIrradPerFA
                        + -0.0316492395333113 * getFormFactor() * irrPerFloorArea().ElementAt(2)
                        + 0.113116119215289 * getWfRatio() * irrPerFloorArea().ElementAt(2);
                    break;
            }
         
            if (energyNeed < 0)
            {
                energyNeed = 0;
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
            return getTotalFloorArea() / getTotalEnvelopeArea();
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
                vector.Unitize();

                // North
                if (vector.X <= Math.Sin(45) && vector.Y > 0.0 && vector.Z < 0.5)
                {
                    irradiation[0] += point.irradiationValue;
                    irradiation[5] += point.irradiationValue;
                    irradiation[6] += point.irradiationValue;
                    continue;
                }
                // South
                else if (vector.X < Math.Sin(45) && vector.Y < 0.0 && vector.Z < 0.5)
                {
                    irradiation[1] += point.irradiationValue;
                    irradiation[5] += point.irradiationValue;
                    irradiation[6] += point.irradiationValue;
                    continue;
                }
                // East
                else if (vector.X > 0.0 && vector.Y < Math.Sin(45) && vector.Z < 0.5)
                {
                    irradiation[2] += point.irradiationValue;
                    irradiation[5] += point.irradiationValue;
                    irradiation[6] += point.irradiationValue;
                    continue;
                }
                // West
                else if (vector.X < 0.0 && vector.Y < Math.Sin(45) && vector.Z < 0.5)
                {
                    irradiation[3] += point.irradiationValue;
                    irradiation[5] += point.irradiationValue;
                    irradiation[6] += point.irradiationValue;
                    continue;
                }
                // Top
                else if (vector.Z >= 0.5)
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
                vector.Unitize();

                // Find the corresponding plane
                // North
                if (vector.X <= Math.Sin(45) && vector.Y > 0.0 && vector.Z < 0.5)
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
                if (vector.X < Math.Sin(45) && vector.Y < 0.0 && vector.Z < 0.5)
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
                if (vector.X > 0.0 && vector.Y < Math.Sin(45) && vector.Z < 0.5)
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
                if (vector.X < 0.0 && vector.Y < Math.Sin(45) && vector.Z < 0.5)
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
            return description.glazingRatio * getTotalFacadeArea() / getTotalFloorArea();
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
                    vector.Unitize();

                    // Find the corresponding plane
                    // North
                    if (vector.X <= Math.Sin(45) && vector.Y > 0.0 && vector.Z < 0.5)
                    {
                        areas[0] += facade.GetArea();
                        continue;
                    }

                    // South
                    if (vector.X < Math.Sin(45) && vector.Y < 0.0 && vector.Z < 0.5)
                    {
                        areas[1] += facade.GetArea();
                        continue;
                    }

                    // East
                    if (vector.X > 0.0 && vector.Y < Math.Sin(45) && vector.Z < 0.5)
                    {
                        areas[2] += facade.GetArea();
                        continue;
                    }

                    // West
                    if (vector.X < 0.0 && vector.Y < Math.Sin(45) && vector.Z < 0.5)
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

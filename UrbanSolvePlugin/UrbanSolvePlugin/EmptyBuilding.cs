using Rhino.Geometry;
using System;
using System.Linq;

namespace UrbanSolvePlugin
{
    class EmptyBuilding : Building
    {
        public double depth { get; set; }

        public EmptyBuilding(EmptyDescription description, Point3d refPoint, double length, double width, 
            int numberOfFloors, double depth, double storeyHeight, double rotation, bool isCentered) : 
                base(description, refPoint, length, width, numberOfFloors, storeyHeight, rotation, isCentered)
        {
            this.depth = depth;
            geometry = new EmptyGeometry(this);
        }

        public override string ToString()
        {
            string text = EnumMethods.GetDescriptionFromEnumValue(Typology.emptySquare) + Environment.NewLine
            + "Length: " + length + " (min " + description.minLength + ", max " + description.maxLength + ")\r"
            + "Width: " + width + " (min " + description.minWidth + ", max " + description.maxWidth + ")\r"
            + "Depth: " + depth + " (min " + ((EmptyDescription)description).minDepth + ", max " + ((EmptyDescription)description).maxDepth + ")\r"
            + "Storey height: " + storeyHeight + "\r"
            + "Storey number: " + numberOfFloors + " (min " + description.minStoreyNumber + ", max " + description.maxStoreyNumber + ")\r"
            + "Rotation: " + Rhino.RhinoMath.ToDegrees(rotation) + "\r"
            + "Position: " + refPoint.X + " / " + refPoint.Y;
            return text;
        }

        public override string ToCsv()
        {
            return length + "-" + width + "-" + depth + "-" + numberOfFloors;
        }

        public override double getDaylightAutonomy(double floorAreaRatio, double siteCoverage)
        {
            double buildingPerformance = 1061.14900259261
                + 52.8916082652949 * siteCoverage
                + 0.00199526375729272 * getTotalFloorArea()     // ExpEnvArea
                +-0.0335130538020271 * getFootprintArea()
                + 0.00258539965422525 * getTotalFloorArea()
                + -6.60055233629505 * numberOfFloors
                + -1155.60205221466 * getFormFactor()
                + 1589.91363011607 * getRoofRatio()
                + -83.3467654206859 * getNorthFacRatio()
                + -205.148821959631 * getEastFacRatio()
                + -19.5983417670361 * description.glazingRatio
                + 271.11838929568 * getWfRatio()
                + -2.82941478730075 * getMeanIrradiation().ElementAt(5)     // MeanEnvelopeIrrad
                + -0.690849907630037 * getMeanIrradiation().ElementAt(4)    // MeanRoofIrrad
                + 2.83212026728038 * getMeanIrradiation().ElementAt(6)      // MeanFacIrrad
                + -0.178756147281987 * getMeanIrradiation().ElementAt(0)    //  MeanNorthFacIrrad
                + -0.0942901356875502 * getMeanIrradiation().ElementAt(3)   // MeanWestFacIrrad
                + 3.23183800605235 * irrPerFloorArea().ElementAt(0)         // NorthFacIrradPerFA
                + 0.550561607001849 * irrPerFloorArea().ElementAt(3)        // WestFacIrradPerFA
                + 0.870032416531565 * getFormFactor() * getMeanIrradiation().ElementAt(4) // FormFactor:MeanRoofIrrad
                + -92.8166645678044 * description.glazingRatio * getWfRatio() // WWRatio:WFRatio
                + -0.00523257038310425 * getMeanIrradiation().ElementAt(6) * irrPerFloorArea().ElementAt(0); // MeanFacIrrad:NorthFacIrradPerFA

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

        public override double getTotalFacadeArea()
        {
            double facadeArea = 0.0;
            facadeArea += length * storeyHeight * numberOfFloors * 2.0;
            facadeArea += width * storeyHeight * numberOfFloors * 2.0;
            facadeArea += (length - depth * 2.0) * storeyHeight * numberOfFloors * 2.0;
            facadeArea += (width - depth * 2.0) * storeyHeight * numberOfFloors * 2.0;
            return facadeArea;
        }

        public override double getFootprintArea()
        {
            double floorArea = 0.0;
            floorArea += length * depth * 2.0;
            floorArea += (width - 2.0 * depth) * depth * 2.0;
            return floorArea;
        }
    }
}

﻿using Rhino.Geometry;
using System;
using System.Linq;

namespace UrbanSolvePlugin
{
    class LShapedBuilding : Building
    {
        public double depth { get; set; }

        public LShapedBuilding(LShapedDescription description, Point3d refPoint, double length, double width,
            int numberOfFloors, double depth, double storeyHeight, double rotation, bool isCentered) : 
                base(description, refPoint, length, width, numberOfFloors, storeyHeight, rotation, isCentered)
        {
            this.depth = depth;
            this.geometry = new LShapedGeometry(this);
        }

        public override string ToString()
        {     
            string text = EnumMethods.GetDescriptionFromEnumValue(Typology.lShaped) + Environment.NewLine
            + "Length: " + length + " (min " + description.minLength + ", max " + description.maxLength + ")\r"
            + "Width: " + width + " (min " + description.minWidth + ", max " + description.maxWidth + ")\r"
            + "Depth: " + depth + " (min " + ((LShapedDescription)description).minDepth + ", max " + ((LShapedDescription)description).maxDepth + ")\r"
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
            /*
            double buildingPerformance = 298.867463420089
                + 37.3140894656246 * floorAreaRatio
                + 0.0110661612440121 * getTotalEnvelopeArea() // ExpEnvArea
                +0.0120005785869003 * getFootprintArea()
                + -0.00387799368392111 * getTotalFloorArea()
                + -2.35124386049808 * numberOfFloors
                + -2.99223871308239 * depth
                + -186.73576943947 * getRoofRatio()
                + 181.229401109214 * description.glazingRatio
                + 495.355140316642 * getWfRatio()
                + -0.0848187438772535 * getMeanIrradiation().ElementAt(4) // MeanRoofIrrad
                + 0.208155017683284 * getMeanIrradiation().ElementAt(6) // MeanFacIrrad
                + -1.54325270417759 * getMeanIrradiation().ElementAt(0)  // MeanNorthFacIrrad
                + -0.228438849108589 * getMeanIrradiation().ElementAt(3) // MeanWestFacIrrad
                + 0.139314859777534 * irrPerFloorArea().ElementAt(4) // RoofIrradPerFA
                + -0.540259882444968 * irrPerFloorArea().ElementAt(6) // FacIrradPerFA
                + 0.445587664049494 * irrPerFloorArea().ElementAt(0) // NorthFacIrradPerFA
                + 0.23103381370352 * irrPerFloorArea().ElementAt(3) // WestFacIrradPerFA
                + -0.0596125056151693 * floorAreaRatio * getMeanIrradiation().ElementAt(6) // PlotRatio:MeanFacIrrad
                + -0.0000351723790821589 * getTotalEnvelopeArea() * irrPerFloorArea().ElementAt(3) // ExpEnvArea:WestFacIrradPerFA
                + -0.00046263694359697 * getFootprintArea() * irrPerFloorArea().ElementAt(0)// FootprintArea:NorthFacIrradPerFA
                + -14.6175590642624 * depth * description.glazingRatio // LargestDimension:WWRatio
                + 0.033987071363019 * depth * getMeanIrradiation().ElementAt(0) // LargestDimension:MeanNorthFacIrrad
                + -0.212360248674878 * getRoofRatio() * irrPerFloorArea().ElementAt(6) // RoofRatio:FacIrradPerFA
                + -271.052819500799 * description.glazingRatio * getWfRatio() // WWRatio:WFRatio
                + 0.68714355361114 * description.glazingRatio * getMeanIrradiation().ElementAt(0) // WWRatio:MeanNorthFacIrrad
                + -1.38282790920061 * getWfRatio() * getMeanIrradiation().ElementAt(0) // WFRatio:MeanNorthFacIrrad
                + 0.000768692005176429 * getMeanIrradiation().ElementAt(0) * getMeanIrradiation().ElementAt(3) // MeanNorthFacIrrad:MeanWestFacIrrad
                + 0.00196725498068676 * getMeanIrradiation().ElementAt(0) * irrPerFloorArea().ElementAt(6); // MeanNorthFacIrrad:FacIrradPerFA
                */

            double buildingPerformance = 222.145661066745
                + 0.000220258249055163 * getTotalEnvelopeArea()
                + -0.00160802593360915 * getTotalFloorArea()
                + -4.32709921709497 * depth
                + -28.9489988668622 * getFormFactor()
                + -7.08511269029049 * description.glazingRatio
                + 663.380401952884 * getWfRatio()
                + -0.0887888421002073 * getMeanIrradiation().ElementAt(4) // MeanRoofIrrad
                + -1.0137632022171 * getMeanIrradiation().ElementAt(0)  // MeanNorthFacIrrad
                + 0.0245153568611562 * irrPerFloorArea().ElementAt(5) // EnvelopeIrradPerFA
                + -0.379577572288953 * irrPerFloorArea().ElementAt(6) // FacIrradPerFA
                + 0.110502802804067 * irrPerFloorArea().ElementAt(3) // WestFacIrradPerFA
                + 0.0107442487354075 * getTotalEnvelopeArea() * description.glazingRatio
                + -0.0207573228789212 * getTotalEnvelopeArea() * getWfRatio()
                + 0.0000138706957905543 * getTotalEnvelopeArea() * irrPerFloorArea().ElementAt(6)
                + -0.0000217362847518072 * getTotalEnvelopeArea() * irrPerFloorArea().ElementAt(3)
                + -9.87212655581741 * depth * description.glazingRatio
                + -15.7135529980318 * depth * getWfRatio()
                + 0.0402625072877102 * depth * getMeanIrradiation().ElementAt(0)
                + -237.855476513946 * description.glazingRatio * getWfRatio()
                + 0.216828582372739 * description.glazingRatio * getMeanIrradiation().ElementAt(4)
                + -0.452059871377729 * getWfRatio() * getMeanIrradiation().ElementAt(0)
                + -0.358005438933464 * getWfRatio() * irrPerFloorArea().ElementAt(6)
                + 0.00182460771866584 * getMeanIrradiation().ElementAt(0) * irrPerFloorArea().ElementAt(6);

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
            facadeArea += length * storeyHeight * numberOfFloors;
            facadeArea += (length - depth) * storeyHeight * numberOfFloors;
            return facadeArea;
        }

        public override double getFootprintArea()
        {
            double footprintArea = 0.0;
            footprintArea += length * depth;
            footprintArea += (width - depth) * depth;
            return footprintArea;
        }
    }
}

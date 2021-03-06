﻿using System;
using Rhino.Geometry;
using System.Linq;

namespace UrbanSolvePlugin
{
    class SquareBuilding : Building
    {
        public SquareBuilding(SquareDescription description, Point3d refPoint, double length, double width,
            int numberOfFloors, double storeyHeight, double rotation, bool isCentered) : 
                base(description, refPoint, length, width, numberOfFloors, storeyHeight, rotation, isCentered)
        {
            geometry = new SquareGeometry(this);
        }

        public override string ToString()
        {
            string text = EnumMethods.GetDescriptionFromEnumValue(Typology.square) + Environment.NewLine
                + "Length: " + length + " (min " + description.minLength + ", max " + description.maxLength + ")\r"
                + "Width: " + width + " (min " + description.minWidth + ", max " + description.maxWidth + ")\r"
                + "Storey height: " + storeyHeight + "\r"
                + "Storey number: " + numberOfFloors + " (min " + description.minStoreyNumber + ", max " + description.maxStoreyNumber + ")\r"
                + "Rotation: " + Rhino.RhinoMath.ToDegrees(rotation) + "\r"
                + "Position: " + refPoint.X + " / " + refPoint.Y;
            return text;
        }

        public override string ToCsv()
        {
            return length + "-" + width + "-" + numberOfFloors;
        }

        public override double getDaylightAutonomy(double floorAreaRatio, double siteCoverage)
        {
            /*
            double buildingPerformance = -7.37646067643102
                + -3.7609973845904 * floorAreaRatio
                + 648.462390824602 * siteCoverage
                + 0.0432477611772784 * getFootprintArea()
                + -96.8225571273825 * getFormFactor()
                + -301.808234397105 * getWestFacRatio() // westfacaderatio
                +-186.616624228871 * description.glazingRatio
                + 553.010890609635 * getWfRatio()
                + -0.227163674639613 * getMeanIrradiation().ElementAt(5) // MeanEnvelopeIrrad
                +0.117800381468841 * getMeanIrradiation().ElementAt(4)
                + 0.309105973435479 * getMeanIrradiation().ElementAt(6)
                + -0.1287983259025 * getMeanIrradiation().ElementAt(0) // MeanNorthFacIrrad
                + -0.0835216758516964 * getMeanIrradiation().ElementAt(2)// MeanEastFacIrrad
                + -0.0590148547521802 * getMeanIrradiation().ElementAt(3) // MeanWestFacIrrad
                + -0.414019492621023 * irrPerFloorArea().ElementAt(6) // FacIrradPerFA
                + 0.762616239364522 * irrPerFloorArea().ElementAt(2) // EastFacIrradPerFA
                + 0.0822337407389414 * irrPerFloorArea().ElementAt(1) // SouthFacIrradPerFA
                + 0.0371028548751901 * irrPerFloorArea().ElementAt(3) // WestFacIrradPerFA
                + -0.538875257102462 * siteCoverage * getMeanIrradiation().ElementAt(4) // SiteCoverage:MeanRoofIrrad
                + 0.35633798467971 * getFormFactor() * getMeanIrradiation().ElementAt(0) // FormFactor:MeanNorthFacIrrad
                + 0.129709180683937 * getFormFactor() * irrPerFloorArea().ElementAt(1) // FormFactor:SouthFacIrradPerFA
                + -117.206044634487 * description.glazingRatio * getWfRatio() // WWRatio:WFRatio
                +0.272775952151501 * description.glazingRatio * getMeanIrradiation().ElementAt(6) // WWRatio:MeanFacIrrad
                + -0.175312569343854 * description.glazingRatio * getMeanIrradiation().ElementAt(0) // WWRatio:MeanNorthFacIrrad
                + -0.443345047333225 * getWfRatio() * irrPerFloorArea().ElementAt(6) // WFRatio:FacIrradPerFA
                + 0.000537443917736514 * getMeanIrradiation().ElementAt(3) * irrPerFloorArea().ElementAt(3) // MeanWestFacIrrad:WestFacIrradPerFA
                + 0.000343474633894671 * irrPerFloorArea().ElementAt(6) * irrPerFloorArea().ElementAt(1); // FacIrradPerFA:
                */

            double buildingPerformance = 47.9831314059456
                + -0.00753913503604804 * getTotalEnvelopeArea() // ExpEnvArea
                + 0.0187869114532609 * getFootprintArea()
                + 0.015115298688913 * getTotalFloorArea()
                + -94.755841608948 * getFormFactor()
                + 24.1143352390221 * getRoofRatio()
                + -71.3860691438678 * description.glazingRatio
                + 409.817551528917 * getWfRatio()
                + -0.0496179399291882 * getMeanIrradiation().ElementAt(4) // MeanRoofIrrad
                + -0.182715966268388 * getMeanIrradiation().ElementAt(6) // MeanFacIrrad
                + -0.0560173501723925 * getMeanIrradiation().ElementAt(0) // MeanNorthFacIrrad
                + 0.0376599059150131 * irrPerFloorArea().ElementAt(5) // EnvelopeIrradPerFA
                + 0.235144279563239 * irrPerFloorArea().ElementAt(6) // FacIrradPerFA
                + -0.00204031592890172 * irrPerFloorArea().ElementAt(1) // SouthFacIrradPerFA
                + -0.000035262497865225 * getTotalEnvelopeArea() * getFootprintArea()
                + 0.000180073437343323 * getFootprintArea() * getMeanIrradiation().ElementAt(6)
                + -189.651315093995 * description.glazingRatio * getWfRatio()
                + 0.282259467033907 * getWfRatio() * getMeanIrradiation().ElementAt(6)
                + -0.367858178534162 + getWfRatio() * irrPerFloorArea().ElementAt(6)
                + 0.000183371862509953 * getMeanIrradiation().ElementAt(0) * irrPerFloorArea().ElementAt(5);

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

        public override double getFootprintArea()
        {
            return length * width;
        }

        public override double getTotalFacadeArea()
        {
            double facadeArea = 0.0;
            facadeArea += length * numberOfFloors * storeyHeight * 2;
            facadeArea += width * numberOfFloors * storeyHeight * 2;
            return facadeArea;
        }
    }
}

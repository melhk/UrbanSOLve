using Rhino.Geometry;
using System;

namespace UrbanSolvePlugin
{
    /// <summary>
    /// Classe de base pour la description d'un bâtiment.
    /// </summary>
    public abstract class BuildingDescription
    {
        protected Point3d refPoint;
        public Typology typology { get; protected set; }
        public Program program { get; private set; }
        public double glazingRatio { get; private set; }
        // Initial length given by the user
        public double initialLength { get; private set; }
        public double minLength { get; private set; }
        public double maxLength { get; private set; }
        // Initial width given by the user
        public double initialWidth { get; private set; }
        public double minWidth { get; private set; }
        public double maxWidth { get; private set; }
        public int initialStoreyNumber { get; private set; }
        public int minStoreyNumber { get; private set; }
        public int maxStoreyNumber { get; private set; }
        public double storeyHeight { get; private set; }
        // Radians
        public double rotation { get; private set; }
        public bool isCentered { get; private set; }

        protected BuildingDescription(Point3d refPoint, Program program, double glazingRatio,
            double minLength, double maxLength, double initialLength,
            double minWidth, double maxWidth, double initialWidth, int minStoreyNumber, 
            int maxStoreyNumber, int initialStoreyNumber,
            double storeyHeight, double rotation, bool isCentered)
        {
            this.program = program;
            this.glazingRatio = glazingRatio;
            this.refPoint = refPoint;
            this.minLength = minLength;
            this.maxLength = maxLength;
            this.initialLength = initialLength;
            this.minWidth = minWidth;
            this.maxWidth = maxWidth;
            this.initialWidth = initialWidth;
            this.minStoreyNumber = minStoreyNumber;
            this.maxStoreyNumber = maxStoreyNumber;
            this.initialStoreyNumber = initialStoreyNumber;
            this.storeyHeight = storeyHeight;
            this.isCentered = isCentered;
            this.rotation = Rhino.RhinoMath.ToRadians(- rotation);
        }

        public override string ToString()
        {
            string text = "";
            text += "Typology: " + EnumMethods.GetDescriptionFromEnumValue(typology) + Environment.NewLine;
            text += "Rotation: " + rotation + Environment.NewLine;
            text += "Alignment: " + (isCentered ? "Center" : "Corner") + Environment.NewLine;
            text += String.Format("Length: {0} (min {1}, max {2})" + Environment.NewLine, initialLength, minLength, maxLength);
            text += String.Format("Width: {0} (min {1}, max {2})" + Environment.NewLine, initialWidth, minWidth, maxWidth);
            text += String.Format("Number of floors: {0} (min {1}, max {2})" + Environment.NewLine, initialStoreyNumber, minStoreyNumber, maxStoreyNumber);
            text += "Function: " + EnumMethods.GetDescriptionFromEnumValue(program) + Environment.NewLine;
            text += "Window-to-wall ratio: " + glazingRatio + Environment.NewLine;
            text += String.Format("Position: {0} / {1}", (double)decimal.Round((decimal)refPoint.X, 2, MidpointRounding.AwayFromZero), 
                (double)decimal.Round((decimal)refPoint.Y, 2, MidpointRounding.AwayFromZero));
            return text;
        }

        public abstract DescriptionGeometry getBuildingDescriptionGeometry();
        public abstract int getNumberOfVariables();
        public abstract Building getBuilding(double[] values);
        public abstract Building getStartBuilding();
        public abstract double[] getLowerBounds();
        public abstract double[] getUpperBounds();
        public abstract double[] getActualValues();
    }
}

using Rhino.Geometry;
using System;

namespace UrbanSolvePlugin
{
    class EmptyDescription : BuildingDescription
    {
        private const int numberOfVariables = 4;
        public double minDepth { get; private set; }
        public double maxDepth { get; private set; }
        public double initialDepth { get; private set; }

        public EmptyDescription(Point3d refPoint, Program program, double glazingRatio, double minLength, double maxLength, double startLength,
            double minWidth, double maxWidth, double startWidth, int minStoreyNumber, int maxStoreyNumber, int startStoreyNumber,
            double minDepth, double maxDepth, double startDepth, double storeyHeight, double rotation, bool isCentered) :
            base(refPoint, program, glazingRatio, minLength, maxLength, startLength, minWidth, maxWidth, startWidth, minStoreyNumber, 
                maxStoreyNumber, startStoreyNumber, storeyHeight, rotation, isCentered)
        {
            this.minDepth = minDepth;
            this.maxDepth = maxDepth;
            this.initialDepth = startDepth;
            this.typology = Typology.emptySquare;
        }

        public override string ToString()
        {
            string text = "";
            text += "Typology: " + EnumMethods.GetDescriptionFromEnumValue(typology) + Environment.NewLine;
            text += "Rotation: " + rotation + Environment.NewLine;
            text += "Alignment: " + (isCentered ? "Center" : "Corner") + Environment.NewLine;
            text += String.Format("Length: {0} (min {1}, max {2})" + Environment.NewLine, initialLength, minLength, maxLength);
            text += String.Format("Width: {0} (min {1}, max {2})" + Environment.NewLine, initialWidth, minWidth, maxWidth);
            text += String.Format("Depth: {0} (min {1}, max {2})" + Environment.NewLine, initialDepth, minDepth, maxDepth);
            text += String.Format("Number of floors: {0} (min {1}, max {2})" + Environment.NewLine, initialStoreyNumber, minStoreyNumber, maxStoreyNumber);
            text += "Function: " + EnumMethods.GetDescriptionFromEnumValue(program) + Environment.NewLine;
            text += "Window-to-wall ratio: " + glazingRatio + Environment.NewLine;
            text += String.Format("Position: {0} / {1}", (double)decimal.Round((decimal)refPoint.X, 2, MidpointRounding.AwayFromZero),
                (double)decimal.Round((decimal)refPoint.Y, 2, MidpointRounding.AwayFromZero));
            return text;
        }

        public override Building getBuilding(double[] values)
        {
            try
            {
                return new EmptyBuilding(this, refPoint, values[0], values[1], Convert.ToInt32(values[2]), values[3],
                    storeyHeight, rotation, isCentered);
            } catch (IndexOutOfRangeException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                throw;
            }
        }

        public override DescriptionGeometry getBuildingDescriptionGeometry()
        {
            EmptyGeometry actual = new EmptyGeometry(initialLength, initialWidth, initialDepth, initialStoreyNumber, storeyHeight, rotation, isCentered, refPoint);
            return new DescriptionGeometry(refPoint, "E", actual.geometry);
        }

        public override int getNumberOfVariables()
        {
            return numberOfVariables;
        }

        public override Building getStartBuilding()
        {
            return new EmptyBuilding(this, refPoint, initialLength, initialWidth, initialStoreyNumber, initialDepth, storeyHeight, rotation, isCentered);
        }

        public override double[] getLowerBounds()
        {
            double[] lowerBounds = new double[numberOfVariables];
            lowerBounds.SetValue(minLength, 0);
            lowerBounds.SetValue(minWidth, 1);
            lowerBounds.SetValue(minStoreyNumber, 2);
            lowerBounds.SetValue(minDepth, 3);
            return lowerBounds;
        }

        public override double[] getUpperBounds()
        {
            double[] upperBounds = new double[numberOfVariables];
            upperBounds.SetValue(maxLength, 0);
            upperBounds.SetValue(maxWidth, 1);
            upperBounds.SetValue(maxStoreyNumber, 2);
            upperBounds.SetValue(maxDepth, 3);
            return upperBounds;
        }

        public override double[] getActualValues()
        {
            double[] actualValues = new double[numberOfVariables];
            actualValues.SetValue(initialLength, 0);
            actualValues.SetValue(initialWidth, 1);
            actualValues.SetValue(initialStoreyNumber, 2);
            actualValues.SetValue(initialDepth, 3);
            return actualValues;
        }
    }
}

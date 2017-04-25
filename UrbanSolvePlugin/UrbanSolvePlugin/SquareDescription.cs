using Rhino.Geometry;
using System;

namespace UrbanSolvePlugin
{
    class SquareDescription : BuildingDescription
    {
        private const int numberOfVariables = 3;

        public SquareDescription(Point3d refPoint, Program program, double glazingRatio, double minLength, double maxLength, double startLength,
            double minWidth, double maxWidth, double startWidth, int minStoreyNumber, int maxStoreyNumber, int startStoreyNumber, double storeyHeight, double rotation, bool isCentered) : 
            base(refPoint, program, glazingRatio, minLength, maxLength, startLength, minWidth, maxWidth, startWidth, minStoreyNumber, maxStoreyNumber, startStoreyNumber, storeyHeight, rotation, isCentered)
        {
            typology = Typology.square;
        }

        public override double[] getLowerBounds()
        {
            double[] lowerBounds = new double[numberOfVariables];
            lowerBounds.SetValue(minLength, 0);
            lowerBounds.SetValue(minWidth, 1);
            lowerBounds.SetValue(minStoreyNumber, 2);
            return lowerBounds;
        }

        public override double[] getUpperBounds()
        {
            double[] upperBounds = new double[numberOfVariables];
            upperBounds.SetValue(maxLength, 0);
            upperBounds.SetValue(maxWidth, 1);
            upperBounds.SetValue(maxStoreyNumber, 2);
            return upperBounds;
        }

        public override Building getBuilding(double[] values)
        {
            // Check table length!
            return new SquareBuilding(this, refPoint, values[0], values[1], Convert.ToInt32(values[2]), storeyHeight, rotation, isCentered);
        }

        public override Building getStartBuilding()
        {
            return new SquareBuilding(this, refPoint, initialLength, initialWidth, initialStoreyNumber, storeyHeight, rotation, isCentered);
        }

        public override DescriptionGeometry getBuildingDescriptionGeometry()
        {
            SquareGeometry actual = new SquareGeometry(initialLength, initialWidth, initialStoreyNumber, storeyHeight, rotation, isCentered, refPoint);
            return new DescriptionGeometry(refPoint, "S", actual.geometry);
        }

        public override double[] getActualValues()
        {
            double[] actualValues = new double[numberOfVariables];
            actualValues.SetValue(initialLength, 0);
            actualValues.SetValue(initialWidth, 1);
            actualValues.SetValue(initialStoreyNumber, 2);
            return actualValues;
        }

        public override int getNumberOfVariables()
        {
            return numberOfVariables;
        }
    }
}

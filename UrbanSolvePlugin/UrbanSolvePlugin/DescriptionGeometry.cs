using Rhino.Geometry;

namespace UrbanSolvePlugin
{
    public class DescriptionGeometry
    {
        public string text { get; private set; }
        public Point3d refPoint { get; private set; }
        public Brep startGeometry { get; private set; }

        public DescriptionGeometry(Point3d refPoint, string text, Brep startGeometry)
        {
            this.text = text;
            this.refPoint = refPoint;
            this.startGeometry = startGeometry;
        }
    }
}

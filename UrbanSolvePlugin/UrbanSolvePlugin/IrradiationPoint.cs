using Rhino.Geometry;

namespace UrbanSolvePlugin
{
    public class IrradiationPoint
    {
        public Point3d node { get; private set; }
        public Vector3d vector { get; private set; }
        public double irradiationValue { get; set; }

        public IrradiationPoint(Point3d node, Vector3d vector)
        {
            this.node = node;
            this.vector = vector;
        }
    }
}

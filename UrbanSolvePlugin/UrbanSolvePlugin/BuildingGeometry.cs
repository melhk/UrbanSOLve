using Rhino.Geometry;
using System.Collections.Generic;

namespace UrbanSolvePlugin
{
    public abstract class BuildingGeometry
    {
        public Brep geometry { get; protected set; }

        public List<Brep> getFacades()
        {
            List<Brep> facadeList = new List<Brep>();
            Brep dup = geometry.DuplicateBrep();
            Vector3d roofVector = Vector3d.ZAxis;
            Vector3d floorVector = roofVector;
            floorVector.Reverse();

            Rhino.Geometry.Collections.BrepFaceList faces = dup.Faces;
            int nbFaces = faces.Count;

            for (int i = 0; i < nbFaces; ++i)
            {
                Vector3d normal = faces[i].NormalAt(0.5, 0.5);
                if (!(1 == normal.IsParallelTo(roofVector)) && !(1 == normal.IsParallelTo(floorVector)))
                {
                    facadeList.Add(faces.ExtractFace(i));
                }
            }
            return facadeList;
        }
    }
}
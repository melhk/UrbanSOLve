using System.Collections.Generic;
using Rhino.Display;
using Rhino.Geometry;


namespace UrbanSolvePlugin
{
    public class DescriptionConduit : DisplayConduit
    {
        public List<BuildingDescription> descriptions { private get; set; }

        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            base.CalculateBoundingBox(e);
            if (null != descriptions)
            {
                foreach (BuildingDescription descr in descriptions)
                {
                    DescriptionGeometry descriptionGeometry = descr.getBuildingDescriptionGeometry();
                    e.BoundingBox.Union(descriptionGeometry.startGeometry.GetBoundingBox(false));
                }
            }
        }

        protected override void PostDrawObjects(DrawEventArgs e)
        {
            base.PostDrawObjects(e);
            
            if (null != descriptions)
            {
                DisplayMaterial material = new DisplayMaterial(CST.DESCRIPTION_COLOR_3D);
                material.IsTwoSided = false;
                e.Display.EnableLighting(true);

                int i = 1;
                foreach (BuildingDescription description in descriptions)
                {
                    DescriptionGeometry descriptionGeometry = description.getBuildingDescriptionGeometry();
                    material.Transparency = CST.MATERIAL_TRANSPARENCY;
                    e.Display.DrawBrepShaded(descriptionGeometry.startGeometry, material);
                    e.Display.DrawPoint(descriptionGeometry.refPoint, PointStyle.Simple, CST.CENTER_POINT_SIZE, CST.DESCRIPTION_COLOR_WHITE);

                    Plane plane = new Plane();
                    Point3d textPosition = descriptionGeometry.refPoint;
                    textPosition.Z += description.storeyHeight * description.initialStoreyNumber + 1;

                    plane.Origin = descriptionGeometry.refPoint;
                    // Figure out the size. This means measuring the visible size in the viewport AT 
                    // the current location.
                    double pixPerUnit;
                    e.Viewport.GetFrustumFarPlane(out plane);
                    e.Viewport.GetWorldToScreenScale(descriptionGeometry.refPoint, out pixPerUnit);

                    double textSize = CST.TEXT_SIZE / pixPerUnit;

                    using (Text3d drawText = new Text3d(i.ToString(), plane, textSize))
                    {
                        Point3d centerPoint = drawText.BoundingBox.PointAt(0.5, 0.5, 0.0);
                        Point3d drawingPoint = Point3d.Add(centerPoint, -textPosition);
                        e.Display.Draw3dText(drawText, CST.DESCRIPTION_COLOR_3D_TEXT, textPosition);
                    }
                    ++i;
                }
            }
        }
    }
}

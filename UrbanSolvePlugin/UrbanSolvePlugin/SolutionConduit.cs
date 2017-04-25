using Rhino.Display;
using Rhino.Geometry;
using System;


namespace UrbanSolvePlugin
{
    class SolutionConduit : DisplayConduit
    {
        public Variant variant { private get; set; }

        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            base.CalculateBoundingBox(e);
            if (null != variant)
            {
                foreach (Brep brep in variant.getGeometry())
                {
                    e.BoundingBox.Union(brep.GetBoundingBox(false));
                }
            }           
        }

        protected override void PostDrawObjects(DrawEventArgs e)
        {
            base.PostDrawObjects(e);
            
            if (null != variant)
            {
                DisplayMaterial material = new DisplayMaterial(CST.SOLUTION_COLOR_3D);
                material.Transparency = CST.MATERIAL_TRANSPARENCY;
                material.IsTwoSided = false;
                e.Display.EnableLighting(true);

                int i = 1;
                foreach (Building b in variant.buildings)
                {
                    e.Display.DrawPoint(b.refPoint, PointStyle.Simple, CST.CENTER_POINT_SIZE, CST.SOLUTION_COLOR_3D_TEXT);
                    e.Display.DrawBrepShaded(b.geometry.geometry, material);

                    Plane plane = new Plane();
                    Point3d textPosition = b.refPoint;
                    textPosition.Z += b.storeyHeight * b.numberOfFloors + 1; 

                    plane.Origin = b.refPoint;
                    // Figure out the size. This means measuring the visible size in the viewport AT the current location.
                    double pixPerUnit;
                    e.Viewport.GetFrustumFarPlane(out plane);
                    e.Viewport.GetWorldToScreenScale(b.refPoint, out pixPerUnit);

                    double textSize = CST.TEXT_SIZE / pixPerUnit;

                    using (Text3d drawText = new Text3d(i.ToString(), plane, textSize))
                    {
                        Point3d centerPoint = drawText.BoundingBox.PointAt(0.5, 0.5, 0.0);
                        Point3d drawingPoint = Point3d.Add(-centerPoint, textPosition);
                        e.Display.Draw3dText(drawText, CST.SOLUTION_COLOR_3D_TEXT, textPosition);
                    }
                    ++i;
                }

                foreach (Building building in variant.buildings)
                {
                    foreach (IrradiationPoint p in building.irradiationPoints)
                    {
                        e.Display.DrawPoint(p.node, PointStyle.Simple, CST.IRRAD_POINT_SIZE,
                            getColor(p.irradiationValue, CST.MIN_IRR_VALUE, CST.MAX_IRR_VALUE));
                    }
                }
            }
        }

        private System.Drawing.Color getColor(double irradiationValue, double min, double max)
        {
            int red = 0;
            int green = 0;
            int blue = 0;

            if (irradiationValue < min)
            {
                irradiationValue = min;
            }
            else if (irradiationValue > max)
            {
                irradiationValue = max;
            }

            double normValue = ((irradiationValue - min) / (max - min)) * 100;

            if (0 <= normValue && normValue <= 33)
            {
                normValue = normValue * 3;
                red = 0;
                green = (int)Math.Round(normValue) * 255 / 100;
                blue = 255;
            }
            else if (33 < normValue && normValue <= 66)
            {
                normValue = (normValue - 33) * 3;
                red = (int)Math.Round(normValue) * 255 / 100;
                green = 255;
                blue = 255 - ((int)Math.Round(normValue) * 255 / 100);
            }
            else if (66 < normValue && normValue <= 100)
            {
                normValue = (normValue - 66) * 3;
                red = 255;
                green = 255 - ((int)Math.Round(normValue) * 255 / 100);
                blue = 0;
            }
            else if (normValue > 100)
            {
                red = 255;
                green = 0;
                blue = 0;
            }
            return System.Drawing.Color.FromArgb(red, green, blue);
        }
    }
}
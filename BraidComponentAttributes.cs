using Grasshopper.Kernel;
using Grasshopper.GUI.Canvas;
using System.Drawing;
using Grasshopper.GUI;
using Grasshopper.Kernel.Attributes;
using System;
using System.Linq;
using Grasshopper.Kernel.Special;
using Grasshopper.GUI.Base;

namespace _3D_Braid
{
    public class BraidComponentAttributes : GH_ComponentAttributes
    {
        private readonly BraidComponent owner;
        private PointF lastPosition;

        public BraidComponentAttributes(BraidComponent component)
            : base(component)
        {
            this.owner = component;
            this.lastPosition = Pivot;
        }

        protected override void Layout()
        {
            base.Layout();
            Rectangle bounds = GH_Convert.ToRectangle(Bounds);
            bounds.Height += 15;
            Bounds = bounds;

            if (lastPosition != Pivot)
            {
                float deltaX = Pivot.X - lastPosition.X;
                float deltaY = Pivot.Y - lastPosition.Y;

                owner.RecordUndoEvent("Move Objects");
                UpdateConnectedObjects(deltaX, deltaY);
                lastPosition = Pivot;

                GH_Document doc = owner.OnPingDocument();
                if (doc != null)
                {
                    doc.DestroyAttributeCache();
                    doc.ScheduleSolution(10);
                }
            }
        }

        private void UpdateConnectedObjects(float deltaX, float deltaY)
        {
            if (owner?.Params?.Input == null) return;

            foreach (IGH_Param param in owner.Params.Input)
            {
                if (param.Sources != null)
                {
                    foreach (IGH_Param source in param.Sources)
                    {
                        if (source?.Attributes != null)
                        {
                            var currentPivot = source.Attributes.Pivot;
                            source.Attributes.Pivot = new PointF(
                                currentPivot.X + deltaX,
                                currentPivot.Y + deltaY
                            );
                            source.Attributes.ExpireLayout();
                        }
                    }
                }
            }
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            base.Render(canvas, graphics, channel);
        }
    }
}
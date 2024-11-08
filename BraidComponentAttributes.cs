using Grasshopper.Kernel;
using Grasshopper.GUI.Canvas;
using System.Drawing;
using Grasshopper.GUI;
using Grasshopper.Kernel.Attributes;
using System;

namespace _3D_Braid
{
    public class BraidComponentAttributes : GH_ComponentAttributes
    {
        private readonly BraidComponent owner;

        public BraidComponentAttributes(BraidComponent component)
            : base(component)
        {
            this.owner = component;
        }

        protected override void Layout()
        {
            base.Layout();
            Rectangle bounds = GH_Convert.ToRectangle(Bounds);
            bounds.Height += 30;
            Bounds = bounds;
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            base.Render(canvas, graphics, channel);
        }
    }
}
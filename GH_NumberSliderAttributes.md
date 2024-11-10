using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Expressions;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Undo;
using Microsoft.VisualBasic.CompilerServices;

namespace Grasshopper.Kernel.Special;

public class GH_NumberSliderAttributes : GH_ResizableAttributes<GH_NumberSlider>
{
    private class SliderDragUndo : GH_ObjectUndoAction
    {
        private decimal _value;

        //
        // Сводка:
        //     Override this property if you want the Grasshopper solution to refresh upon undo
        //     completion.
        public override bool ExpiresSolution { get; }

        public SliderDragUndo(GH_NumberSlider owner)
            : base(owner.InstanceGuid)
        {
            _value = owner.Slider.Value;
            ExpiresSolution = true;
        }

        protected override void Object_Undo(GH_Document doc, IGH_DocumentObject obj)
        {
            if (obj is GH_NumberSlider gH_NumberSlider)
            {
                decimal value = gH_NumberSlider.Slider.Value;
                gH_NumberSlider.Slider.Value = _value;
                _value = value;
            }
        }

        protected override void Object_Redo(GH_Document doc, IGH_DocumentObject obj)
        {
            Object_Undo(doc, obj);
        }
    }

    private static readonly int SliderMinWidth = 120;

    private int m_dragmode;

    private string m_cachedName;

    private Rectangle m_boundsName;

    private Rectangle m_boundsSlider;

    protected override Size MinimumSize => new Size(100, 20);

    protected override Size MaximumSize => new Size(5000, 20);

    protected override Padding SizingBorders => new Padding(0, 0, 6, 0);

    public override bool HasInputGrip => false;

    public override bool TooltipEnabled => true;

    public GH_NumberSliderAttributes(GH_NumberSlider nOwner)
        : base(nOwner)
    {
        m_dragmode = 0;
        ExpireLayout();
        Layout();
    }

    protected override void Layout()
    {
        SizeF sizeF = GH_FontServer.MeasureString(base.Owner.ImpliedNickName, GH_FontServer.StandardAdjusted);
        sizeF.Width = Math.Max(sizeF.Width + 10f, MinimumSize.Height);
        Bounds = new RectangleF(Pivot.X, Pivot.Y, Math.Max(Bounds.Width, sizeF.Width + (float)SliderMinWidth), MinimumSize.Height);
        Bounds = GH_Convert.ToRectangle(Bounds);
        m_boundsName = GH_Convert.ToRectangle(new RectangleF(Pivot.X, Pivot.Y, sizeF.Width, MinimumSize.Height));
        m_boundsSlider = Rectangle.FromLTRB(m_boundsName.Right, m_boundsName.Top, Convert.ToInt32(Bounds.Right), m_boundsName.Bottom);
        base.Owner.Slider.Font = GH_FontServer.StandardAdjusted;
        base.Owner.Slider.DrawControlBorder = false;
        base.Owner.Slider.DrawControlShadows = false;
        base.Owner.Slider.DrawControlBackground = false;
        base.Owner.Slider.TickCount = 11;
        base.Owner.Slider.TickFrequency = 5;
        base.Owner.Slider.RailDarkColour = Color.FromArgb(30, Color.Black);
        base.Owner.Slider.TickDisplay = GH_SliderTickDisplay.Simple;
        base.Owner.Slider.RailDisplay = GH_SliderRailDisplay.Simple;
        base.Owner.Slider.Padding = new Padding(6, 2, 6, 1);
        base.Owner.Slider.Bounds = m_boundsSlider;
    }

    public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (e.Button == MouseButtons.Left)
        {
            if ((double)sender.Viewport.Zoom >= 0.9 && m_boundsSlider.Contains(GH_Convert.ToPoint(e.CanvasLocation)))
            {
                string content = base.Owner.Slider.GripTextPure;
                if (base.Owner.IsExpression)
                {
                    GH_ExpressionParser gH_ExpressionParser = new GH_ExpressionParser(bThrowExceptions: false);
                    try
                    {
                        gH_ExpressionParser.CacheSymbols(base.Owner.Expression);
                        gH_ExpressionParser.AddVariable("x", Convert.ToDouble(base.Owner.Slider.Value));
                        GH_Variant gH_Variant = gH_ExpressionParser.Evaluate();
                        if (gH_Variant.IsNumeric)
                        {
                            content = gH_Variant.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        ProjectData.SetProjectError(ex);
                        Exception ex2 = ex;
                        ProjectData.ClearProjectError();
                    }
                }

                base.Owner.Slider.TextInputHandlerDelegate = TextInputHandler;
                base.Owner.Slider.ShowTextInputBox(sender, limitToBoundary: true, sender.Viewport.XFormMatrix(GH_Viewport.GH_DisplayMatrix.CanvasToControl), content);
            }
            else
            {
                base.Owner.PopupEditor();
            }

            return GH_ObjectResponse.Handled;
        }

        return GH_ObjectResponse.Ignore;
    }

    private void TextInputHandler(GH_SliderBase slider, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        try
        {
            if (GH_Convert.ToDouble(text, out var destination, GH_Conversion.Secondary))
            {
                base.Owner.RecordUndoEvent("Slider Value Change");
                base.Owner.TrySetSliderValue(Convert.ToDecimal(destination));
            }
        }
        catch (Exception ex)
        {
            ProjectData.SetProjectError(ex);
            Exception ex2 = ex;
            ProjectData.ClearProjectError();
        }
    }

    public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (base.Owner.Slider.MouseDown(e.WinFormsEventArgs, e.CanvasLocation))
        {
            m_dragmode = 1;
            sender.Invalidate();
            Rectangle rail = base.Owner.Slider.Rail;
            decimal d = new decimal(rail.Right - rail.Left);
            d = decimal.Multiply(d, Convert.ToDecimal(sender.Viewport.Zoom));
            decimal snapDistance = decimal.Multiply(decimal.Divide(decimal.Subtract(base.Owner.Slider.Maximum, base.Owner.Slider.Minimum), d), 10m);
            base.Owner.Slider.SnapDistance = snapDistance;
            return GH_ObjectResponse.Capture;
        }

        return base.RespondToMouseDown(sender, e);
    }

    public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        switch (e.Button)
        {
            case MouseButtons.None:
                if (base.Owner.Slider.Grip.Contains(e.CanvasLocation))
                {
                    Instances.CursorServer.AttachCursor(sender, "GH_NumericSlider");
                    return GH_ObjectResponse.Handled;
                }

                break;
            case MouseButtons.Left:
                if (m_dragmode == 1)
                {
                    base.Owner.TriggerAutoSave(base.Owner.InstanceGuid);
                    base.Owner.RecordUndoEvent("Slider change", new SliderDragUndo(base.Owner));
                    m_dragmode = 2;
                }

                if (m_dragmode == 2)
                {
                    base.Owner.Slider.MouseMove(e.WinFormsEventArgs, e.CanvasLocation);
                    return GH_ObjectResponse.Handled;
                }

                break;
        }

        return base.RespondToMouseMove(sender, e);
    }

    public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        if (m_dragmode > 0)
        {
            m_dragmode = 0;
            base.Owner.Slider.MouseUp(e.WinFormsEventArgs, e.CanvasLocation);
            sender.Invalidate();
            return GH_ObjectResponse.Release;
        }

        return base.RespondToMouseUp(sender, e);
    }

    protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
    {
        GH_CanvasChannel gH_CanvasChannel = channel;
        if (gH_CanvasChannel != GH_CanvasChannel.Objects)
        {
            return;
        }

        string impliedNickName = base.Owner.ImpliedNickName;
        if (m_cachedName == null || !impliedNickName.Equals(m_cachedName, StringComparison.Ordinal))
        {
            m_cachedName = impliedNickName;
            ExpireLayout();
            Layout();
        }

        GH_Viewport viewport = canvas.Viewport;
        RectangleF rec = Bounds;
        bool num = viewport.IsVisible(ref rec, 10f);
        Bounds = rec;
        if (!num)
        {
            return;
        }

        int[] radii = new int[4] { 3, 0, 0, 3 };
        int[] radii2 = new int[4] { 0, 3, 3, 0 };
        GH_Capsule gH_Capsule = null;
        gH_Capsule = base.Owner.RuntimeMessageLevel switch
        {
            GH_RuntimeMessageLevel.Warning => GH_Capsule.CreateTextCapsule(m_boundsName, m_boundsName, GH_Palette.Warning, impliedNickName, radii, 5),
            GH_RuntimeMessageLevel.Error => GH_Capsule.CreateTextCapsule(m_boundsName, m_boundsName, GH_Palette.Error, impliedNickName, radii, 5),
            _ => GH_Capsule.CreateTextCapsule(m_boundsName, m_boundsName, GH_Palette.Hidden, impliedNickName, radii, 5),
        };
        gH_Capsule.Render(graphics, Selected, base.Owner.Locked, hidden: true);
        gH_Capsule.Dispose();
        GH_Capsule gH_Capsule2 = GH_Capsule.CreateCapsule(m_boundsSlider, GH_Palette.Normal, radii2, 5);
        gH_Capsule2.AddOutputGrip(OutputGrip.Y);
        gH_Capsule2.Render(graphics, Selected, base.Owner.Locked, hidden: false);
        gH_Capsule2.Dispose();
        if (!base.Owner.IsExpression || base.Owner.VolatileDataCount == 0)
        {
            base.Owner.Slider.FormatMask = "{0}";
        }
        else
        {
            object objectValue = RuntimeHelpers.GetObjectValue(base.Owner.VolatileData.get_Branch(0)[0]);
            base.Owner.Slider.FormatMask = ((GH_Number)objectValue).Value.ToString();
        }

        int zoomFadeLow = GH_Canvas.ZoomFadeLow;
        if (zoomFadeLow > 0)
        {
            base.Owner.Slider.Render(graphics);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            if (zoomFadeLow < 255)
            {
                GH_PaletteStyle impliedStyle = GH_CapsuleRenderEngine.GetImpliedStyle(GH_Palette.Normal, Selected, base.Owner.Locked, hidden: false);
                SolidBrush solidBrush = new SolidBrush(Color.FromArgb(255 - zoomFadeLow, impliedStyle.Fill));
                Rectangle bounds = base.Owner.Slider.Bounds;
                bounds.X += base.Owner.Slider.Padding.Left;
                bounds.Y += base.Owner.Slider.Padding.Top;
                bounds.Width -= base.Owner.Slider.Padding.Horizontal;
                bounds.Height -= base.Owner.Slider.Padding.Vertical;
                graphics.FillRectangle(solidBrush, bounds);
                solidBrush.Dispose();
            }
        }

        if (m_dragmode > 0 && base.Owner.IsExpression)
        {
            PointF location = base.Owner.Slider.Grip.Location;
            location.X += 0.5f * base.Owner.Slider.Grip.Width;
            RectangleF bounds2 = Bounds;
            bounds2.Inflate(1000f, 1000f);
            GH_GraphicsUtil.RenderBalloonTag(graphics, "Original: " + base.Owner.Slider.GripTextPure, GH_FontServer.Standard, location, bounds2);
        }
    }
}
#region сборка Grasshopper, Version=8.14.24298.18001, Culture=neutral, PublicKeyToken=dda4f5ec2cd80803
// C:\Program Files\Rhino 8\Plug-ins\Grasshopper\Grasshopper.dll
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Base;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI.HTML;
using Grasshopper.GUI.RemotePanel;
using Grasshopper.Kernel.Expressions;
using Grasshopper.Kernel.Types;
using Grasshopper.My.Resources;
using Microsoft.VisualBasic.CompilerServices;
using Rhino.Geometry;

namespace Grasshopper.Kernel.Special;

public class GH_NumberSlider : GH_Param<GH_Number>, IGH_StateAwareObject, IRcpAwareObject, IGH_InitCodeAware
{
    private enum InitNotation
    {
        None,
        DoubleDot,
        Range
    }

    public class GH_NumberSliderPublishProxy : RcpDocumentObjectItem<GH_NumberSlider>
    {
        private readonly GH_SliderBase _slider;

        private bool _down;

        public override int DesiredHeight => Global_Proc.UiAdjust(20);

        public GH_NumberSliderPublishProxy(GH_NumberSlider owner)
            : base(owner)
        {
            _slider = new GH_SliderBase();
            _down = false;
        }

        public override void Render(Graphics G)
        {
            if (base.Owner == null)
            {
                G.SmoothingMode = SmoothingMode.None;
                Pen pen = new Pen(RcpItem.ErrorHint, 2f);
                G.DrawRectangle(pen, base.ClientBounds);
                pen.Dispose();
                SolidBrush solidBrush = new SolidBrush(RcpItem.ErrorHint);
                G.DrawString("Slider missing", GH_FontServer.StandardBold, solidBrush, base.ClientBounds, GH_TextRenderingConstants.NearCenter);
                solidBrush.Dispose();
            }
            else
            {
                SolidBrush solidBrush2 = new SolidBrush(RcpItem.FieldFill);
                G.FillRectangle(solidBrush2, base.ClientBounds);
                solidBrush2.Dispose();
                Pen pen2 = new Pen(RcpItem.NormalEdge, 2f);
                G.DrawRectangle(pen2, base.ClientBounds);
                pen2.Dispose();
                Rectangle clientBounds = base.ClientBounds;
                clientBounds.Inflate(0, 2);
                _slider.Bounds = clientBounds;
                _slider.Type = base.Owner.Slider.Type;
                _slider.FormatMask = base.Owner.Slider.FormatMask;
                _slider.Minimum = base.Owner.Slider.Minimum;
                _slider.Maximum = base.Owner.Slider.Maximum;
                _slider.DecimalPlaces = base.Owner.Slider.DecimalPlaces;
                _slider.Value = base.Owner.Slider.Value;
                _slider.DrawControlBackground = false;
                _slider.DrawControlBorder = false;
                _slider.GripDisplay = GH_SliderGripDisplay.Numeric;
                _slider.RailDisplay = GH_SliderRailDisplay.None;
                _slider.TickDisplay = GH_SliderTickDisplay.None;
                _slider.DrawControlShadows = false;
                Color color = RcpItem.NormalFill;
                Color color2 = RcpItem.NormalEdge;
                if (_down)
                {
                    color = RcpItem.ActiveFill;
                    color2 = RcpItem.ActiveEdge;
                }

                _slider.GripTopColour = color;
                _slider.GripBottomColour = color;
                _slider.GripEdgeColour = color2;
                _slider.TextColour = color2;
                string impliedNickName = base.Owner.ImpliedNickName;
                if (!string.IsNullOrWhiteSpace(impliedNickName))
                {
                    SolidBrush solidBrush3 = new SolidBrush(RcpItem.NormalEdge);
                    int num = GH_FontServer.StringWidth(impliedNickName, GH_FontServer.StandardBold);
                    if ((float)checked(base.TextBounds.X + num + 10) < _slider.Grip.Left)
                    {
                        G.DrawString(impliedNickName, GH_FontServer.StandardBold, solidBrush3, base.TextBounds, GH_TextRenderingConstants.NearCenter);
                    }
                    else
                    {
                        G.DrawString(impliedNickName, GH_FontServer.StandardBold, solidBrush3, base.TextBounds, GH_TextRenderingConstants.FarCenter);
                    }

                    solidBrush3.Dispose();
                }

                _slider.Render(G);
            }

            G.SmoothingMode = SmoothingMode.HighQuality;
        }

        private void SliderValueChanged(object sender, GH_SliderEventArgs e)
        {
            _slider.ValueChanged -= SliderValueChanged;
            if (base.Owner != null)
            {
                base.Owner.Slider.Value = e.Slider.Value;
                base.Owner.ExpireSolution(recompute: true);
            }
        }

        public override GH_ObjectResponse MouseDown(MouseEventArgs e)
        {
            if (_slider.MouseDown(e, e.Location))
            {
                _down = true;
                return GH_ObjectResponse.Capture;
            }

            return base.MouseDown(e);
        }

        public override GH_ObjectResponse MouseMove(MouseEventArgs e)
        {
            _slider.ValueChanged += SliderValueChanged;
            bool num = _slider.MouseMove(e, e.Location);
            _slider.ValueChanged -= SliderValueChanged;
            if (num)
            {
                return GH_ObjectResponse.Capture;
            }

            return base.MouseMove(e);
        }

        public override GH_ObjectResponse MouseUp(MouseEventArgs e)
        {
            _down = false;
            if (_slider.MouseUp(e, e.Location))
            {
                return GH_ObjectResponse.Release;
            }

            return base.MouseUp(e);
        }
    }

    private readonly GH_SliderBase m_slider;

    private string m_expression;

    private readonly List<string> _snapRanges;

    //
    // Сводка:
    //     Gets the internal slider instance. Do not muck about with this unless you know
    //     what you are doing.
    public GH_SliderBase Slider => m_slider;

    //
    // Сводка:
    //     Gets the total number of ticks (unique positions) available on the slider. The
    //     number of ticks is limited to one billion (1,000,000,000)
    public int TickCount
    {
        get
        {
            int num = 1000000000;
            int num2 = 1;
            decimal value = decimal.Subtract(m_slider.Maximum, m_slider.Minimum);
            num2 = ((Slider.Type != 0) ? Convert.ToInt32(value) : Convert.ToInt32(Convert.ToDouble(value) * Math.Pow(10.0, Math.Min(Slider.DecimalPlaces, 5))));
            if (num2 >= num)
            {
                return num;
            }

            return Convert.ToInt32(num2);
        }
    }

    //
    // Сводка:
    //     Gets or sets the current slider value as an offset in tick-space.
    public int TickValue
    {
        get
        {
            double normalizedValue = Slider.NormalizedValue;
            int tickCount = TickCount;
            return Math.Min(Math.Max(Convert.ToInt32(normalizedValue * (double)tickCount), 0), tickCount);
        }
        set
        {
            int tickCount = TickCount;
            decimal d = Convert.ToDecimal((double)value / (double)tickCount);
            SetSliderValue(decimal.Add(Slider.Minimum, decimal.Multiply(d, decimal.Subtract(Slider.Maximum, Slider.Minimum))));
        }
    }

    //
    // Сводка:
    //     Gets the current slider value. This includes any expressions that might be active.
    //     Call this property if you want to know the current value.
    public decimal CurrentValue
    {
        get
        {
            decimal num = Slider.Value;
            if (IsExpression)
            {
                GH_ExpressionParser gH_ExpressionParser = new GH_ExpressionParser(bThrowExceptions: false);
                try
                {
                    gH_ExpressionParser.CacheSymbols(Expression);
                    gH_ExpressionParser.AddVariable("x", Convert.ToDouble(num));
                    GH_Variant gH_Variant = gH_ExpressionParser.Evaluate();
                    switch (gH_Variant.Type)
                    {
                        case GH_VariantType.@bool:
                            num = (gH_Variant._Bool ? 1m : 0m);
                            break;
                        case GH_VariantType.@int:
                            num = new decimal(gH_Variant._Int);
                            break;
                        case GH_VariantType.@double:
                            num = Convert.ToDecimal(gH_Variant._Double);
                            break;
                    }

                    num = GH_SliderBase.ProcessNumber(num, Slider.Type, Slider.DecimalPlaces);
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Exception ex2 = ex;
                    ProjectData.ClearProjectError();
                }
            }

            return num;
        }
    }

    //
    // Сводка:
    //     Gets or sets the expression for this slider.
    public string Expression
    {
        get
        {
            return m_expression;
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                m_expression = null;
            }
            else
            {
                m_expression = value;
            }
        }
    }

    //
    // Сводка:
    //     Gets whether an expression has been assigned to this slider.
    public bool IsExpression => !string.IsNullOrWhiteSpace(m_expression);

    public override bool IconCapableUI => false;

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override Bitmap Icon => Res_ObjectIcons.Obj_NumberSlider_24x24;

    public override string InstanceDescription
    {
        get
        {
            List<string> list = new List<string>();
            decimal minimum = Slider.Minimum;
            decimal maximum = Slider.Maximum;
            decimal value = Slider.Value;
            decimal num = decimal.Multiply(100m, decimal.Divide(decimal.Subtract(value, minimum), decimal.Subtract(maximum, minimum)));
            switch (Slider.Type)
            {
                case GH_SliderAccuracy.Float:
                    list.Add("Floating point accuracy");
                    break;
                case GH_SliderAccuracy.Integer:
                    list.Add("Integer accuracy");
                    break;
                case GH_SliderAccuracy.Even:
                    list.Add("Even number accuracy");
                    break;
                case GH_SliderAccuracy.Odd:
                    list.Add("Odd number accuracy");
                    break;
            }

            list.Add($"Lower limit: {FormatNumber(minimum, Slider.Type, Slider.DecimalPlaces)}");
            list.Add($"Upper limit: {FormatNumber(maximum, Slider.Type, Slider.DecimalPlaces)}");
            list.Add($"Value: {FormatNumber(value, Slider.Type, Slider.DecimalPlaces)}");
            list.Add($"Factor: {num:0}%");
            return string.Join(Environment.NewLine, list.ToArray());
        }
    }

    //
    // Сводка:
    //     Gets the implied nickname for this slider object. The implied nickname is just
    //     like the regular nickname, except when the regular nickname is a zero-length
    //     string and there are one or more recipient parameter, in which case the nickname
    //     is inherited from the recipients.
    public string ImpliedNickName
    {
        get
        {
            if (NickName.Length > 0)
            {
                return NickName;
            }

            if (Recipients.Count == 0)
            {
                return "Slider";
            }

            string result = "Slider";
            double num = double.MaxValue;
            checked
            {
                int num2 = Recipients.Count - 1;
                for (int i = 0; i <= num2; i++)
                {
                    double num3 = GH_GraphicsUtil.DistanceS(base.Attributes.OutputGrip, Recipients[i].Attributes.InputGrip);
                    if (num3 < num)
                    {
                        num = num3;
                        result = Recipients[i].Name;
                    }
                }

                return result;
            }
        }
    }

    public override GH_ParamKind Kind => GH_ParamKind.floating;

    public override GH_ParamData DataType => GH_ParamData.local;

    public override string TypeName => "Number";

    //
    // Сводка:
    //     Gets the Slider Component Guid.
    public static Guid SliderGuid => new Guid("{57DA07BD-ECAB-415d-9D86-AF36D7073ABC}");

    public override Guid ComponentGuid => SliderGuid;

    //
    // Сводка:
    //     Gets all valid sticky values as intervals. This automatically adds errors to
    //     the slider if a sticky text isn't valid.
    private List<SliderSnapRange> SnappingData
    {
        get
        {
            List<SliderSnapRange> list = new List<SliderSnapRange>();
            Interval rc = default(Interval);
            foreach (string snapRange in _snapRanges)
            {
                double destination;
                if (snapRange.ToUpperInvariant().Contains("TO"))
                {
                    if (GH_Convert.ToInterval(snapRange, ref rc, GH_Conversion.Both))
                    {
                        list.Add(new SliderSnapRange(Convert.ToDecimal(rc.Min), Convert.ToDecimal(rc.Max)));
                    }
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Sticky value '{snapRange}' is not a valid range");
                    }
                }
                else if (GH_Convert.ToDouble(snapRange, out destination, GH_Conversion.Both))
                {
                    list.Add(new SliderSnapRange(Convert.ToDecimal(destination)));
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Sticky value '{snapRange}' is not a valid number");
                }
            }

            return list;
        }
    }

    public GH_NumberSlider()
        : base((IGH_InstanceDescription)new GH_InstanceDescription("Number Slider", string.Empty, "Numeric slider for single values", "Params", "Input"))
    {
        m_slider = new GH_SliderBase();
        m_expression = null;
        _snapRanges = new List<string>();
        m_slider.Type = GH_SliderAccuracy.Float;
        m_slider.Minimum = 0m;
        m_slider.Maximum = 1m;
        m_slider.Value = 0.25m;
        m_slider.DecimalPlaces = 3;
        m_slider.GripDisplay = GH_SliderGripDisplay.ShapeAndText;
        m_slider.Font = GH_FontServer.StandardAdjusted;
        m_slider.ValueChanged += InternalSliderValueChanged;
    }

    public void SetInitCode(string code)
    {
        try
        {
            code = code.Trim();
            GH_Variant gH_Variant = GH_Convert.ParseExpression(code, recursive: true);
            if (gH_Variant.Type != 0)
            {
                switch (gH_Variant.Type)
                {
                    case GH_VariantType.@int:
                        {
                            int @int = gH_Variant._Int;
                            Slider.Type = GH_SliderAccuracy.Integer;
                            if (@int == 0)
                            {
                                Slider.Minimum = 0m;
                                Slider.Maximum = 100m;
                                Slider.Value = 0m;
                            }
                            else if (@int < 0)
                            {
                                @int = Math.Max(@int, -100000000);
                                Slider.Minimum = Convert.ToDecimal(GH_Convert.ToPrevPowerOfTen(@int));
                                Slider.Maximum = 0m;
                                Slider.Value = new decimal(@int);
                            }
                            else
                            {
                                @int = Math.Min(@int, 100000000);
                                Slider.Minimum = 0m;
                                Slider.Maximum = Convert.ToDecimal(GH_Convert.ToNextPowerOfTen(@int));
                                Slider.Value = new decimal(@int);
                            }

                            goto end_IL_0000;
                        }
                    case GH_VariantType.@double:
                        {
                            double @double = gH_Variant._Double;
                            Slider.Type = GH_SliderAccuracy.Float;
                            int num = HarvestDecimalPlaces(code);
                            if (num == 0)
                            {
                                num = 3;
                            }

                            Slider.DecimalPlaces = num;
                            if (@double == 0.0)
                            {
                                Slider.Minimum = 0m;
                                Slider.Maximum = 1m;
                                Slider.Value = 0m;
                            }
                            else if (@double < 0.0)
                            {
                                @double = Math.Max(@double, -100000000.0);
                                Slider.Minimum = Convert.ToDecimal(GH_Convert.ToPrevPowerOfTen(@double));
                                Slider.Maximum = 0m;
                                Slider.Value = Convert.ToDecimal(@double);
                            }
                            else
                            {
                                @double = Math.Min(@double, 100000000.0);
                                Slider.Minimum = 0m;
                                Slider.Maximum = Convert.ToDecimal(GH_Convert.ToNextPowerOfTen(@double));
                                Slider.Value = Convert.ToDecimal(@double);
                            }

                            goto end_IL_0000;
                        }
                }
            }

            string minimum = null;
            string maximum = null;
            string value = null;
            if (HarvestRange(code, out minimum, out maximum, out value))
            {
                GH_Variant gH_Variant2 = GH_Convert.ParseExpression(minimum, recursive: true);
                GH_Variant gH_Variant3 = GH_Convert.ParseExpression(maximum, recursive: true);
                GH_Variant gH_Variant4 = GH_Convert.ParseExpression(value, recursive: true);
                if (gH_Variant2.Type == GH_VariantType.@int && gH_Variant3.Type == GH_VariantType.@int && gH_Variant4.Type == GH_VariantType.@int)
                {
                    Slider.Type = GH_SliderAccuracy.Integer;
                    Slider.DecimalPlaces = 3;
                    Slider.Minimum = new decimal(Math.Min(gH_Variant2._Int, gH_Variant3._Int));
                    Slider.Maximum = new decimal(Math.Max(gH_Variant2._Int, gH_Variant3._Int));
                    Slider.Value = new decimal(gH_Variant4._Int);
                }
                else
                {
                    int val = 0;
                    val = Math.Max(val, HarvestDecimalPlaces(minimum));
                    val = Math.Max(val, HarvestDecimalPlaces(maximum));
                    val = Math.Max(val, HarvestDecimalPlaces(value));
                    if (val == 0)
                    {
                        val = 3;
                    }

                    Slider.Type = GH_SliderAccuracy.Float;
                    Slider.DecimalPlaces = val;
                    Slider.Minimum = Convert.ToDecimal(Math.Min(gH_Variant2._Double, gH_Variant3._Double));
                    Slider.Maximum = Convert.ToDecimal(Math.Max(gH_Variant2._Double, gH_Variant3._Double));
                    Slider.Value = Convert.ToDecimal(gH_Variant4._Double);
                }
            }

        end_IL_0000:;
        }
        catch (Exception ex)
        {
            ProjectData.SetProjectError(ex);
            Exception ex2 = ex;
            ProjectData.ClearProjectError();
        }

        Slider.FixDomain();
        Slider.FixValue();
    }

    void IGH_InitCodeAware.SetInitCode(string code)
    {
        //ILSpy generated this explicit interface implementation from .override directive in SetInitCode
        this.SetInitCode(code);
    }

    //
    // Сводка:
    //     Try and harvest the number of decimal places implied by a piece of text. Text
    //     must represent a regular number.
    //
    // Параметры:
    //   text:
    //     Text to parse.
    //
    // Возврат:
    //     The number of decimal places (though never more than 16) or 0 if no decimal places
    //     could be harvested.
    public static int HarvestDecimalPlaces(string text)
    {
        text = text.Trim();
        if (text.Length == 0)
        {
            return 0;
        }

        bool flag = false;
        int num = 0;
        checked
        {
            int num2 = text.Length - 1;
            for (int i = 0; i <= num2; i++)
            {
                char c = text[i];
                if (i == 0 && (c == '-' || c == '+'))
                {
                    continue;
                }

                if (c == '.')
                {
                    if (flag)
                    {
                        return 0;
                    }

                    flag = true;
                    continue;
                }

                if (!char.IsDigit(c))
                {
                    return 0;
                }

                if (flag)
                {
                    num++;
                }
            }

            return Math.Min(num, 16);
        }
    }

    //
    // Сводка:
    //     Parse an init code for validity. Valid init codes are A<B, A<B<C, A..B, A..B..C
    //     Where A, B and C can all be evaluated to doubles or integers. This method does
    //     not validate whether the actual values of A, B and C are non-decremental. Note
    //     that sliders can also be initiated using init codes that evaluate to single numbers.
    //     This method does not take those into account. This is purely for validating range
    //     inits.
    //
    // Параметры:
    //   text:
    //     Init code to parse.
    //
    //   minimum:
    //     If text is a valid init code, the minimum descriptor will be returned here.
    //
    //   maximum:
    //     If text is a valid init code, the maximum descriptor will be returned here.
    //
    //   value:
    //     If text is a valid init code with a value portion, the value descriptor will
    //     be returned here.
    //
    // Возврат:
    //     True if the text is a valid init code.
    public static bool HarvestRange(string text, out string minimum, out string maximum, out string value)
    {
        minimum = string.Empty;
        maximum = string.Empty;
        value = string.Empty;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        string[] array = new string[3]
        {
            string.Empty,
            string.Empty,
            string.Empty
        };
        int num = 0;
        InitNotation initNotation = InitNotation.None;
        bool flag = false;
        int num2 = 0;
        checked
        {
            int num3 = text.Length - 1;
            for (int i = 0; i <= num3; i++)
            {
                char c = text[i];
                if (c == '"')
                {
                    flag = !flag;
                    array[num] += Conversions.ToString(c);
                    continue;
                }

                if (flag)
                {
                    array[num] += Conversions.ToString(c);
                    continue;
                }

                switch (c)
                {
                    case '(':
                    case '[':
                    case '{':
                        num2++;
                        array[num] += Conversions.ToString(c);
                        continue;
                    case ')':
                    case ']':
                    case '}':
                        num2--;
                        if (num2 < 0)
                        {
                            return false;
                        }

                        array[num] += Conversions.ToString(c);
                        continue;
                }

                if (num2 > 0)
                {
                    array[num] += Conversions.ToString(c);
                    continue;
                }

                switch (c)
                {
                    case '<':
                        if (initNotation == InitNotation.DoubleDot)
                        {
                            return false;
                        }

                        initNotation = InitNotation.Range;
                        num++;
                        if (num > 2)
                        {
                            return false;
                        }

                        continue;
                    case '.':
                        {
                            int num4 = i + 1;
                            int num5 = text.Length - 1;
                            int j;
                            for (j = num4; j <= num5 && text[j] == '.'; j++)
                            {
                            }

                            j--;
                            if (j <= i)
                            {
                                break;
                            }

                            if (initNotation == InitNotation.Range)
                            {
                                return false;
                            }

                            initNotation = InitNotation.DoubleDot;
                            num++;
                            if (num > 2)
                            {
                                return false;
                            }

                            i = j;
                            continue;
                        }
                }

                array[num] += Conversions.ToString(c);
            }

            if (initNotation == InitNotation.None)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(array[0]))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(array[num]))
            {
                num--;
            }

            if (num < 0)
            {
                return false;
            }

            int num6 = array.Length - 1;
            for (int k = 0; k <= num6; k++)
            {
                if (!string.IsNullOrWhiteSpace(array[k]))
                {
                    GH_Variant gH_Variant = GH_Convert.ParseExpression(array[k], recursive: true);
                    if (gH_Variant.Type != GH_VariantType.@double && gH_Variant.Type != GH_VariantType.@int)
                    {
                        return false;
                    }
                }
            }

            switch (num)
            {
                case 0:
                    {
                        if (GH_Convert.ToDouble(array[0], out var destination, GH_Conversion.Secondary))
                        {
                            minimum = array[0].Trim();
                            if (destination == 0.0)
                            {
                                maximum = "1.0";
                            }
                            else if (destination < 0.0)
                            {
                                maximum = GH_Convert.ToPrevPowerOfTen(destination).ToString();
                            }
                            else
                            {
                                maximum = GH_Convert.ToNextPowerOfTen(destination).ToString();
                            }

                            value = array[0].Trim();
                        }

                        break;
                    }
                case 1:
                    minimum = array[0].Trim();
                    maximum = array[1].Trim();
                    value = array[0].Trim();
                    break;
                case 2:
                    minimum = array[0].Trim();
                    maximum = array[2].Trim();
                    value = array[1].Trim();
                    break;
            }

            return true;
        }
    }

    private void InternalSliderValueChanged(object sender, GH_SliderEventArgs e)
    {
        ExpireSolution(recompute: true);
    }

    public override void CreateAttributes()
    {
        m_attributes = new GH_NumberSliderAttributes(this);
    }

    protected override void CollectVolatileData_Custom()
    {
        Slider.FixDomain();
        Slider.FixValue();
        m_data.Clear();
        if (!IsExpression)
        {
            m_data.Append(new GH_Number(Convert.ToDouble(CurrentValue)));
            return;
        }

        string expression = GH_ExpressionSyntaxWriter.RewriteForEvaluator(m_expression);
        try
        {
            GH_ExpressionParser gH_ExpressionParser = new GH_ExpressionParser();
            gH_ExpressionParser.CacheSymbols(expression);
            gH_ExpressionParser.ClearVariables();
            gH_ExpressionParser.AddVariable("x", Convert.ToDouble(Slider.Value));
            GH_Variant gH_Variant = gH_ExpressionParser.Evaluate();
            if (gH_Variant.IsNumeric)
            {
                m_data.Append(new GH_Number(gH_Variant._Double));
                return;
            }

            m_data.Append(new GH_Number(double.NaN));
            base.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Expression did not return a numeric value");
            return;
        }
        catch (Exception ex)
        {
            ProjectData.SetProjectError(ex);
            Exception ex2 = ex;
            ProjectData.ClearProjectError();
        }

        try
        {
            GH_ExpressionParser gH_ExpressionParser2 = new GH_ExpressionParser();
            gH_ExpressionParser2.CacheSymbols(expression);
            gH_ExpressionParser2.ClearVariables();
            gH_ExpressionParser2.AddVariable(NickName, Convert.ToDouble(Slider.Value));
            GH_Variant gH_Variant2 = gH_ExpressionParser2.Evaluate();
            if (gH_Variant2.IsNumeric)
            {
                m_data.Append(new GH_Number(gH_Variant2._Double));
            }
            else
            {
                m_data.Append(new GH_Number(double.NaN));
                base.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Expression did not return a numeric value");
            }

            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Expression uses Slider Name, please change it to \"x\"");
        }
        catch (Exception ex3)
        {
            ProjectData.SetProjectError(ex3);
            Exception ex4 = ex3;
            m_data.Append(new GH_Number(double.NaN));
            base.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Expression generated an error: {ex4.Message}");
            ProjectData.ClearProjectError();
        }
    }

    //
    // Сводка:
    //     Try and set the slider position in such a way as to result in the given value.
    //     If the slider contains an expression this expression will be back-solved (if
    //     possible).
    //
    // Параметры:
    //   target:
    //     Target value to achieve.
    //
    // Возврат:
    //     True indicating success, false indicating failure.
    public bool TrySetSliderValue(decimal target)
    {
        if (!IsExpression)
        {
            Slider.Value = target;
            Slider.FixValue();
            return true;
        }

        double t = 0.0;
        double v = 0.0;
        if (!GH_Convert.BackSolveExpression(Expression, "x", Convert.ToDouble(target), Convert.ToDouble(Slider.Minimum), Convert.ToDouble(Slider.Maximum), 10, out t, out v))
        {
            return false;
        }

        Slider.Value = Convert.ToDecimal(t);
        Slider.FixValue();
        return true;
    }

    //
    // Сводка:
    //     Sets member value directly. This sets the value of the internal slider, it does
    //     not backsolve for any expressions.
    //
    // Параметры:
    //   val:
    //     Value to set.
    public void SetSliderValue(decimal val)
    {
        val = Math.Max(val, m_slider.Minimum);
        val = Math.Min(val, m_slider.Maximum);
        if (decimal.Compare(Slider.Value, val) != 0)
        {
            Slider.RaiseEvents = false;
            Slider.Value = val;
            Slider.RaiseEvents = true;
            ExpireSolution(recompute: false);
        }
    }

    protected internal override string HtmlHelp_Source()
    {
        GH_HtmlFormatter gH_HtmlFormatter = new GH_HtmlFormatter(this);
        gH_HtmlFormatter.Title = "Number slider";
        gH_HtmlFormatter.Description = "A slider is a special interface object that allows for quick setting of individual numeric values. You can change the values and properties through the menu, or by double-clicking a slider object. Sliders can be made longer or shorter by dragging the rightmost edge left or right. Note that sliders only have output grips. <BR> <BR>" + Environment.NewLine + "Sliders appear automatically in the Grasshopper Panel.";
        gH_HtmlFormatter.ContactURI = "https://discourse.mcneel.com/";
        gH_HtmlFormatter.AddRemark("Slider properties can be changed through the context menu or the slider editor. Double click the slider object (but not on the grip) to open the editor.");
        gH_HtmlFormatter.AddRemark("Sliders can be used to generate an animation of the Grasshopper Document. The Animate… feature is available through the slider menu. The animation is a series of still images that represent the solution at different values for the slider. You can specify the resolution of the output, which viewport should be used, how many frames you want and even some tagging and filename formats. The numeric domain of the animation is always the numeric domain of the slider itself.");
        gH_HtmlFormatter.AddRemark("At present it is not possible to render the animation, only viewport captures are possible.", GH_HtmlFormatterPalette.Red);
        return gH_HtmlFormatter.HtmlFormat();
    }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
    {
        ToolStripMenuItem toolStripMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Slider type");
        GH_DocumentObject.Menu_AppendItem(toolStripMenuItem.DropDown, "Floating point", Menu_FloatTypeClicked, enabled: true, Slider.Type == GH_SliderAccuracy.Float);
        GH_DocumentObject.Menu_AppendItem(toolStripMenuItem.DropDown, "Integers", Menu_IntegerTypeClicked, enabled: true, Slider.Type == GH_SliderAccuracy.Integer);
        GH_DocumentObject.Menu_AppendItem(toolStripMenuItem.DropDown, "Odd numbers", Menu_OddTypeClicked, enabled: true, Slider.Type == GH_SliderAccuracy.Odd);
        GH_DocumentObject.Menu_AppendItem(toolStripMenuItem.DropDown, "Even numbers", Menu_EvenTypeClicked, enabled: true, Slider.Type == GH_SliderAccuracy.Even);
        GH_DocumentObject.Menu_AppendSeparator(menu);
        GH_DocumentObject.Menu_AppendItem(menu, "Edit…", Menu_EditClicked);
        GH_DocumentObject.Menu_AppendItem(menu, "Edit Snapping…", Menu_EditSnappingClicked);
        Menu_AppendValueEditor(menu);
        string text = m_expression;
        if (text == null)
        {
            text = string.Empty;
        }

        ToolStripMenuItem toolStripMenuItem2 = GH_DocumentObject.Menu_AppendItem(menu, "Expression");
        GH_DocumentObject.Menu_AppendItem(toolStripMenuItem2.DropDown, "Expression Editor", Menu_ExpressionEditorClick, Res_ContextMenu.Modifier_Expression_16x16);
        GH_DocumentObject.Menu_AppendTextItem(toolStripMenuItem2.DropDown, text, Menu_ExpressionItemKeyDown, null, enabled: true, 200, lockOnFocus: true);
        GH_DocumentObject.Menu_AppendSeparator(menu);
        GH_DocumentObject.Menu_AppendItem(menu, "Animate…", Menu_AnimateSlider);
    }

    private void Menu_FloatTypeClicked(object sender, EventArgs e)
    {
        if (Slider.Type != 0)
        {
            Slider.Type = GH_SliderAccuracy.Float;
            Slider.FixDomain();
            Slider.FixValue();
            ExpireSolution(recompute: true);
        }
    }

    private void Menu_IntegerTypeClicked(object sender, EventArgs e)
    {
        if (Slider.Type != GH_SliderAccuracy.Integer)
        {
            Slider.Type = GH_SliderAccuracy.Integer;
            Slider.FixDomain();
            Slider.FixValue();
            ExpireSolution(recompute: true);
        }
    }

    private void Menu_OddTypeClicked(object sender, EventArgs e)
    {
        if (Slider.Type != GH_SliderAccuracy.Odd)
        {
            Slider.Type = GH_SliderAccuracy.Odd;
            Slider.FixDomain();
            Slider.FixValue();
            ExpireSolution(recompute: true);
        }
    }

    private void Menu_EvenTypeClicked(object sender, EventArgs e)
    {
        if (Slider.Type != GH_SliderAccuracy.Even)
        {
            Slider.Type = GH_SliderAccuracy.Even;
            Slider.FixDomain();
            Slider.FixValue();
            ExpireSolution(recompute: true);
        }
    }

    private void Menu_EditClicked(object sender, EventArgs e)
    {
        PopupEditor();
    }

    private void Menu_EditSnappingClicked(object sender, EventArgs e)
    {
        PopupSnappingEditor();
    }

    private void Menu_ExpressionItemKeyDown(GH_MenuTextBox sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Return:
                {
                    string text = sender.Text;
                    text = (sender.Text = GH_ExpressionSyntaxWriter.RewriteForGraphicInterface(text));
                    if (text.Length == 0)
                    {
                        m_expression = null;
                    }
                    else
                    {
                        m_expression = text;
                    }

                    ExpireSolution(recompute: true);
                    e.Handled = true;
                    break;
                }
            case Keys.Escape:
                sender.CloseEntireMenuStructure();
                e.Handled = true;
                break;
        }
    }

    private void Menu_ExpressionEditorClick(object sender, EventArgs e)
    {
        GH_ExpressionEditor gH_ExpressionEditor = new GH_ExpressionEditor
        {
            PreviewDelegate = ExpressionEditorPreviewButtonClicked,
            Expression = m_expression
        };
        GH_Number gH_Number = m_data.get_FirstItem(filter_nulls: true);
        if (gH_Number == null)
        {
            gH_ExpressionEditor.Variables.Add("x", new GH_Variant(0.0));
        }
        else
        {
            gH_ExpressionEditor.Variables.Add("x", new GH_Variant(gH_Number.Value));
        }

        GH_WindowsFormUtil.CenterFormOnCursor((Form)gH_ExpressionEditor, limitToScreen: true);
        DialogResult dialogResult = gH_ExpressionEditor.ShowDialog(Instances.DocumentEditor);
        if (dialogResult == DialogResult.OK)
        {
            Expression = gH_ExpressionEditor.Expression;
        }

        ExpireSolution(recompute: true);
    }

    private void ExpressionEditorPreviewButtonClicked(string sExpression)
    {
        string expression = m_expression;
        m_expression = sExpression;
        if (m_expression.Length == 0)
        {
            m_expression = null;
        }

        ExpireSolution(recompute: true);
        m_expression = expression;
    }

    public void PopupEditor()
    {
        TriggerAutoSave();
        GH_NumberSliderPopup gH_NumberSliderPopup = new GH_NumberSliderPopup();
        gH_NumberSliderPopup.Setup(this);
        GH_WindowsFormUtil.CenterFormOnCursor((Form)gH_NumberSliderPopup, limitToScreen: true);
        DialogResult dialogResult = gH_NumberSliderPopup.ShowDialog(Instances.DocumentEditor);
        if (dialogResult == DialogResult.OK)
        {
            base.Attributes.ExpireLayout();
            ExpireSolution(recompute: true);
        }
    }

    public void PopupSnappingEditor()
    {
        TriggerAutoSave();
        GH_NumberSliderSnappingEditor gH_NumberSliderSnappingEditor = new GH_NumberSliderSnappingEditor();
        gH_NumberSliderSnappingEditor.SnappingTextBox.Text = string.Join(Environment.NewLine, _snapRanges);
        GH_WindowsFormUtil.CenterFormOnCursor((Form)gH_NumberSliderSnappingEditor, limitToScreen: true);
        DialogResult dialogResult = gH_NumberSliderSnappingEditor.ShowDialog(Instances.DocumentEditor);
        if (dialogResult == DialogResult.OK)
        {
            _snapRanges.Clear();
            _snapRanges.AddRange(gH_NumberSliderSnappingEditor.SnapLines);
            Slider.SetSnapRanges(SnappingData);
            Instances.InvalidateCanvas();
        }
    }

    private void Menu_AppendValueEditor(ToolStripDropDown menu)
    {
        ToolStripMenuItem toolStripMenuItem = GH_DocumentObject.Menu_AppendItem(menu, "Values");
        GH_NumericScrollerValueEditor gH_NumericScrollerValueEditor = new GH_NumericScrollerValueEditor();
        gH_NumericScrollerValueEditor.BackColor = SystemColors.Window;
        if (Slider.Type == GH_SliderAccuracy.Float)
        {
            gH_NumericScrollerValueEditor.SetupValues(Slider.Minimum, Slider.Maximum, Slider.Value, Slider.DecimalPlaces);
        }
        else
        {
            gH_NumericScrollerValueEditor.SetupValues(Slider.Minimum, Slider.Maximum, Slider.Value, 0);
        }

        gH_NumericScrollerValueEditor.OnLimitsChanged += Menu_NumLimitsChanged;
        gH_NumericScrollerValueEditor.OnValueChanged += Menu_NumValueChanged;
        GH_DocumentObject.Menu_AppendCustomItem(toolStripMenuItem.DropDown, gH_NumericScrollerValueEditor, Menu_NumValuesKeyDown, enabled: true, 200, lockOnFocus: true);
        GH_DocumentObject.Menu_AppendSeparator(menu);
    }

    private void Menu_NumLimitsChanged(GH_NumericScrollerValueEditor sender, decimal e_lower, decimal e_upper, decimal e_value)
    {
        Slider.RaiseEvents = false;
        Slider.Minimum = e_lower;
        Slider.Maximum = e_upper;
        Slider.Value = e_value;
        Slider.FixDomain();
        Slider.FixValue();
        Slider.RaiseEvents = true;
        Slider.OnValueChanged(intermediate: false);
        Instances.RedrawCanvas();
    }

    private void Menu_NumValueChanged(GH_NumericScrollerValueEditor sender, decimal e_lower, decimal e_upper, decimal e_value)
    {
        Slider.RaiseEvents = false;
        Slider.Minimum = e_lower;
        Slider.Maximum = e_upper;
        Slider.Value = e_value;
        Slider.RaiseEvents = true;
        Slider.OnValueChanged(intermediate: false);
    }

    private void Menu_NumValuesKeyDown(GH_MenuCustomControl sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Return:
                Slider.OnValueChanged(intermediate: false);
                break;
            case Keys.Cancel:
            case Keys.Escape:
                Slider.OnValueChanged(intermediate: false);
                break;
        }
    }

    private void Menu_AnimateSlider(object sender, EventArgs e)
    {
        GH_SliderAnimator gH_SliderAnimator = new GH_SliderAnimator(this);
        if (gH_SliderAnimator.SetupAnimationProperties())
        {
            try
            {
                gH_SliderAnimator.StartAnimation();
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception ex2 = ex;
                Tracing.Assert(new Guid("{E04C96C0-74E5-4844-9233-5B66CC7C1024}"), "Slider animation failed: " + Environment.NewLine + ex2.Message);
                ProjectData.ClearProjectError();
            }
        }
    }

    //
    // Сводка:
    //     Convert a double precision floating point number into a string, using the rounding
    //     rules as indicated.
    //
    // Параметры:
    //   value:
    //     Number to munge.
    //
    //   accuracy:
    //     Rounding algorithm to use.
    //
    //   digits:
    //     Number of digits, only applies for FloatingPoint rounding.
    public static string FormatNumber(decimal value, GH_SliderAccuracy accuracy, int digits)
    {
        if (accuracy == GH_SliderAccuracy.Float)
        {
            return string.Format("{0:#0." + new string('0', digits) + "}", value);
        }

        return $"{value:#0}";
    }

    public override bool Write(GH_IWriter writer)
    {
        bool result = base.Write(writer);
        GH_IWriter gH_IWriter = writer.CreateChunk("Slider");
        gH_IWriter.SetDouble("Value", Convert.ToDouble(Slider.Value));
        gH_IWriter.SetDouble("Min", Convert.ToDouble(Slider.Minimum));
        gH_IWriter.SetDouble("Max", Convert.ToDouble(Slider.Maximum));
        gH_IWriter.SetInt32("Digits", Slider.DecimalPlaces);
        switch (Slider.Type)
        {
            case GH_SliderAccuracy.Float:
                gH_IWriter.SetInt32("Interval", 0);
                break;
            case GH_SliderAccuracy.Integer:
                gH_IWriter.SetInt32("Interval", 1);
                break;
            case GH_SliderAccuracy.Odd:
                gH_IWriter.SetInt32("Interval", 2);
                break;
            case GH_SliderAccuracy.Even:
                gH_IWriter.SetInt32("Interval", 3);
                break;
        }

        switch (Slider.GripDisplay)
        {
            case GH_SliderGripDisplay.Shape:
                gH_IWriter.SetInt32("GripDisplay", 0);
                break;
            case GH_SliderGripDisplay.ShapeAndText:
                gH_IWriter.SetInt32("GripDisplay", 1);
                break;
            case GH_SliderGripDisplay.Numeric:
                gH_IWriter.SetInt32("GripDisplay", 2);
                break;
        }

        if (!string.IsNullOrEmpty(m_expression))
        {
            gH_IWriter.SetString("Expression", m_expression);
        }

        gH_IWriter.SetInt32("SnapCount", _snapRanges.Count);
        checked
        {
            if (_snapRanges.Count > 0)
            {
                int num = _snapRanges.Count - 1;
                for (int i = 0; i <= num; i++)
                {
                    gH_IWriter.SetString("Snap", i, _snapRanges[i]);
                }
            }

            return result;
        }
    }

    public override bool Read(GH_IReader reader)
    {
        if (!base.Read(reader))
        {
            return false;
        }

        GH_IReader gH_IReader = reader.FindChunk("Slider");
        if (gH_IReader == null)
        {
            reader.AddMessage("Slider property chunk is missing, archive is corrupt.", GH_Message_Type.error);
            return false;
        }

        Slider.DecimalPlaces = gH_IReader.GetInt32("Digits");
        Slider.Minimum = Convert.ToDecimal(gH_IReader.GetDouble("Min"));
        Slider.Maximum = Convert.ToDecimal(gH_IReader.GetDouble("Max"));
        Slider.Value = Convert.ToDecimal(gH_IReader.GetDouble("Value"));
        if (gH_IReader.ItemExists("Expression"))
        {
            m_expression = gH_IReader.GetString("Expression");
        }
        else
        {
            m_expression = null;
        }

        switch (gH_IReader.GetInt32("Interval"))
        {
            case 0:
                Slider.Type = GH_SliderAccuracy.Float;
                break;
            case 1:
                Slider.Type = GH_SliderAccuracy.Integer;
                break;
            case 2:
                Slider.Type = GH_SliderAccuracy.Odd;
                break;
            case 3:
                Slider.Type = GH_SliderAccuracy.Even;
                break;
            default:
                reader.AddMessage("Slider domain type is unrecognized.", GH_Message_Type.warning);
                Slider.Type = GH_SliderAccuracy.Float;
                break;
        }

        Slider.GripDisplay = GH_SliderGripDisplay.ShapeAndText;
        if (gH_IReader.ItemExists("GripDisplay"))
        {
            switch (gH_IReader.GetInt32("GripDisplay"))
            {
                case 0:
                    Slider.GripDisplay = GH_SliderGripDisplay.Shape;
                    break;
                case 1:
                    Slider.GripDisplay = GH_SliderGripDisplay.ShapeAndText;
                    break;
                case 2:
                    Slider.GripDisplay = GH_SliderGripDisplay.Numeric;
                    break;
                default:
                    Slider.GripDisplay = GH_SliderGripDisplay.ShapeAndText;
                    break;
            }
        }

        Slider.FixDomain();
        Slider.FixValue();
        _snapRanges.Clear();
        checked
        {
            if (gH_IReader.ItemExists("SnapCount"))
            {
                int num = gH_IReader.GetInt32("SnapCount") - 1;
                for (int i = 0; i <= num; i++)
                {
                    _snapRanges.Add(gH_IReader.GetString("Snap", i));
                }
            }

            Slider.SetSnapRanges(SnappingData);
            return true;
        }
    }

    public string SaveState()
    {
        GH_LooseChunk gH_LooseChunk = new GH_LooseChunk("slider");
        gH_LooseChunk.SetDouble("Minimum", Convert.ToDouble(Slider.Minimum));
        gH_LooseChunk.SetDouble("Maximum", Convert.ToDouble(Slider.Maximum));
        gH_LooseChunk.SetInt32("Digits", Slider.DecimalPlaces);
        gH_LooseChunk.SetDouble("Value", Convert.ToDouble(Slider.Value));
        return gH_LooseChunk.Serialize_Xml();
    }

    string IGH_StateAwareObject.SaveState()
    {
        //ILSpy generated this explicit interface implementation from .override directive in SaveState
        return this.SaveState();
    }

    public void LoadState(string state)
    {
        GH_LooseChunk gH_LooseChunk = new GH_LooseChunk("slider");
        gH_LooseChunk.Deserialize_Xml(state);
        double value = Convert.ToDouble(Slider.Minimum);
        gH_LooseChunk.TryGetDouble("Minimum", ref value);
        double value2 = Convert.ToDouble(Slider.Maximum);
        gH_LooseChunk.TryGetDouble("Maximum", ref value2);
        int value3 = Slider.DecimalPlaces;
        gH_LooseChunk.TryGetInt32("Digits", ref value3);
        double value4 = Convert.ToDouble(Slider.Value);
        gH_LooseChunk.TryGetDouble("Value", ref value4);
        Slider.RaiseEvents = false;
        Slider.Minimum = Convert.ToDecimal(value);
        Slider.Maximum = Convert.ToDecimal(value2);
        Slider.DecimalPlaces = value3;
        Slider.Value = Convert.ToDecimal(value4);
        Slider.RaiseEvents = true;
        ExpireSolution(recompute: false);
    }

    void IGH_StateAwareObject.LoadState(string state)
    {
        //ILSpy generated this explicit interface implementation from .override directive in LoadState
        this.LoadState(state);
    }

    public IRcpItem PublishRcpItem()
    {
        return new GH_NumberSliderPublishProxy(this);
    }

    IRcpItem IRcpAwareObject.PublishRcpItem()
    {
        //ILSpy generated this explicit interface implementation from .override directive in PublishRcpItem
        return this.PublishRcpItem();
    }
}
#if false // Журнал декомпиляции
Элементов в кэше: "18"
------------------
Разрешить: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Найдена одна сборка: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Загрузить из: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\mscorlib.dll"
------------------
Разрешить: "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Найдена одна сборка: "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Загрузить из: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.dll"
------------------
Разрешить: "Microsoft.VisualBasic, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
Не удалось найти по имени: "Microsoft.VisualBasic, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
------------------
Разрешить: "System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
Найдена одна сборка: "System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
Загрузить из: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.Drawing.dll"
------------------
Разрешить: "Microsoft.CodeAnalysis, Version=4.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
Не удалось найти по имени: "Microsoft.CodeAnalysis, Version=4.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
------------------
Разрешить: "Microsoft.CodeAnalysis.VisualBasic, Version=4.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
Не удалось найти по имени: "Microsoft.CodeAnalysis.VisualBasic, Version=4.6.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
------------------
Разрешить: "System.Collections.Immutable, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
Не удалось найти по имени: "System.Collections.Immutable, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
------------------
Разрешить: "RhinoCommon, Version=8.14.24298.18001, Culture=neutral, PublicKeyToken=552281e97c755530"
Найдена одна сборка: "RhinoCommon, Version=8.14.24298.18001, Culture=neutral, PublicKeyToken=552281e97c755530"
Загрузить из: "C:\Program Files\Rhino 8\System\RhinoCommon.dll"
------------------
Разрешить: "System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Найдена одна сборка: "System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Загрузить из: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.Windows.Forms.dll"
------------------
Разрешить: "GH_IO, Version=8.14.24298.18001, Culture=neutral, PublicKeyToken=6a29997d2e6b4f97"
Найдена одна сборка: "GH_IO, Version=8.14.24298.18001, Culture=neutral, PublicKeyToken=6a29997d2e6b4f97"
Загрузить из: "C:\Program Files\Rhino 8\Plug-ins\Grasshopper\GH_IO.dll"
------------------
Разрешить: "Eto, Version=2.8.0.0, Culture=neutral, PublicKeyToken=552281e97c755530"
Не удалось найти по имени: "Eto, Version=2.8.0.0, Culture=neutral, PublicKeyToken=552281e97c755530"
------------------
Разрешить: "System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Найдена одна сборка: "System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Загрузить из: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.Core.dll"
------------------
Разрешить: "System.Runtime.Caching, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
Не удалось найти по имени: "System.Runtime.Caching, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
------------------
Разрешить: "System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Найдена одна сборка: "System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Загрузить из: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.Xml.dll"
------------------
Разрешить: "Mono.Cecil, Version=0.11.4.0, Culture=neutral, PublicKeyToken=50cebf1cceb9d05e"
Не удалось найти по имени: "Mono.Cecil, Version=0.11.4.0, Culture=neutral, PublicKeyToken=50cebf1cceb9d05e"
------------------
Разрешить: "GH_Util, Version=8.14.24298.18001, Culture=neutral, PublicKeyToken=dda4f5ec2cd80803"
Не удалось найти по имени: "GH_Util, Version=8.14.24298.18001, Culture=neutral, PublicKeyToken=dda4f5ec2cd80803"
------------------
Разрешить: "System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
Не удалось найти по имени: "System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
------------------
Разрешить: "Rhino.UI, Version=8.14.24298.18001, Culture=neutral, PublicKeyToken=552281e97c755530"
Не удалось найти по имени: "Rhino.UI, Version=8.14.24298.18001, Culture=neutral, PublicKeyToken=552281e97c755530"
------------------
Разрешить: "Yak.Core, Version=0.13.9063.32632, Culture=neutral, PublicKeyToken=1a3831826d6621f9"
Не удалось найти по имени: "Yak.Core, Version=0.13.9063.32632, Culture=neutral, PublicKeyToken=1a3831826d6621f9"
------------------
Разрешить: "System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Найдена одна сборка: "System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Загрузить из: "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.Data.dll"
------------------
Разрешить: "Eto.Wpf, Version=2.8.0.0, Culture=neutral, PublicKeyToken=552281e97c755530"
Не удалось найти по имени: "Eto.Wpf, Version=2.8.0.0, Culture=neutral, PublicKeyToken=552281e97c755530"
------------------
Разрешить: "WindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
Не удалось найти по имени: "WindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
------------------
Разрешить: "PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
Не удалось найти по имени: "PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
------------------
Разрешить: "PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
Не удалось найти по имени: "PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
------------------
Разрешить: "System.Numerics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
Не удалось найти по имени: "System.Numerics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
#endif

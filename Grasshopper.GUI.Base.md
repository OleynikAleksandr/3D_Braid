using System;

namespace Grasshopper.GUI.Base;

//
// Сводка:
//     Represents a snap range on a slider.
public struct SliderSnapRange : IComparable<SliderSnapRange>
{
    private readonly decimal _range0;

    private readonly decimal _range1;

    public decimal Min => _range0;

    public decimal Max => _range1;

    public bool IsSingleton => decimal.Compare(_range0, _range1) == 0;

    public SliderSnapRange(decimal value)
    {
        this = default(SliderSnapRange);
        _range0 = value;
        _range1 = value;
    }

    public SliderSnapRange(decimal value0, decimal value1)
    {
        this = default(SliderSnapRange);
        _range0 = Math.Min(value0, value1);
        _range1 = Math.Max(value0, value1);
    }

    //
    // Сводка:
    //     Compute the distance from a value to this snap range.
    public decimal DistanceTo(decimal value)
    {
        if (decimal.Compare(value, _range0) >= 0 && decimal.Compare(value, _range1) <= 0)
        {
            return default(decimal);
        }

        return Math.Min(Math.Abs(decimal.Subtract(value, _range0)), Math.Abs(decimal.Subtract(value, _range1)));
    }

    //
    // Сводка:
    //     Gets the snapped value of another value.
    public decimal SnapValue(decimal value)
    {
        if (decimal.Compare(value, _range0) < 0)
        {
            return _range0;
        }

        if (decimal.Compare(value, _range1) > 0)
        {
            return _range1;
        }

        return value;
    }

    public bool CanMerge(SliderSnapRange other)
    {
        return decimal.Compare(Max, other.Min) >= 0;
    }

    public SliderSnapRange Merge(SliderSnapRange other)
    {
        return new SliderSnapRange(Math.Min(Min, other.Min), Math.Max(Max, other.Max));
    }

    public int CompareTo(SliderSnapRange other)
    {
        return _range0.CompareTo(other._range0);
    }

    int IComparable<SliderSnapRange>.CompareTo(SliderSnapRange other)
    {
        //ILSpy generated this explicit interface implementation from .override directive in CompareTo
        return this.CompareTo(other);
    }
}
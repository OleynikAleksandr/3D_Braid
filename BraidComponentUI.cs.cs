﻿using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace _3D_Braid
{
    public class BraidComponentUI
    {
        private readonly BraidComponent _component;
        private readonly BraidParameters _parameters;
        private RectangleF _dropZone;

        // Константы для UI
        private const float SLIDER_HEIGHT = 20;
        private const float SLIDER_SPACING = 5;
        private const float DROP_ZONE_HEIGHT = 30;
        private const float PADDING = 10;

        private class SliderInfo
        {
            public string Name { get; set; }
            public string NickName { get; set; }
            public double Value { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }
            public RectangleF Bounds { get; set; }
        }

        private List<SliderInfo> _sliders;

        public BraidComponentUI(BraidComponent component, BraidParameters parameters)
        {
            _component = component;
            _parameters = parameters;
            _sliders = new List<SliderInfo>();
            InitializeSliders();
        }

        private void InitializeSliders()
        {
            _sliders = new List<SliderInfo>
            {
                new SliderInfo { Name = "Width", NickName = "W", Value = _parameters.Width, Min = BraidParameters.WIDTH_MIN, Max = BraidParameters.WIDTH_MAX },
                new SliderInfo { Name = "Height", NickName = "H", Value = _parameters.Height, Min = BraidParameters.HEIGHT_MIN, Max = BraidParameters.HEIGHT_MAX },
                new SliderInfo { Name = "Steepness", NickName = "S", Value = _parameters.Steepness, Min = BraidParameters.STEEPNESS_MIN, Max = BraidParameters.STEEPNESS_MAX },
                new SliderInfo { Name = "Points/Period", NickName = "Pts", Value = _parameters.PointsPeriod, Min = BraidParameters.POINTS_MIN, Max = BraidParameters.POINTS_MAX },
                new SliderInfo { Name = "Diameter", NickName = "D", Value = _parameters.Diameter, Min = BraidParameters.DIAMETER_MIN, Max = BraidParameters.DIAMETER_MAX },
                new SliderInfo { Name = "Diameter Offset", NickName = "DO", Value = _parameters.DiameterOffset, Min = BraidParameters.OFFSET_MIN, Max = BraidParameters.OFFSET_MAX },
                new SliderInfo { Name = "Num Periods", NickName = "N", Value = _parameters.NumPeriods, Min = BraidParameters.PERIODS_MIN, Max = BraidParameters.PERIODS_MAX }
            };
        }

        public void LayoutUI(RectangleF bounds)
        {
            float currentY = bounds.Y + PADDING;

            foreach (var slider in _sliders)
            {
                slider.Bounds = new RectangleF(
                    bounds.X + PADDING,
                    currentY,
                    bounds.Width - (2 * PADDING),
                    SLIDER_HEIGHT
                );
                currentY += SLIDER_HEIGHT + SLIDER_SPACING;
            }

            _dropZone = new RectangleF(
                bounds.X + PADDING,
                currentY + SLIDER_SPACING,
                bounds.Width - (2 * PADDING),
                DROP_ZONE_HEIGHT
            );
        }

        public void Render(Graphics graphics, bool selected)
        {
            if (graphics == null) return;

            foreach (var slider in _sliders)
            {
                var rect = new Rectangle(
                    (int)slider.Bounds.X,
                    (int)slider.Bounds.Y,
                    (int)slider.Bounds.Width,
                    (int)slider.Bounds.Height
                );

                // Фон слайдера
                graphics.FillRectangle(
                    selected ? SystemBrushes.Control : SystemBrushes.ControlLight,
                    rect
                );

                // Рамка
                graphics.DrawRectangle(
                    SystemPens.ControlDark,
                    rect
                );

                // Текст
                string text = $"{slider.NickName}: {slider.Value:F2}";
                graphics.DrawString(
                    text,
                    SystemFonts.DefaultFont,
                    SystemBrushes.ControlText,
                    new Point((int)slider.Bounds.X + 5, (int)slider.Bounds.Y + 2)
                );

                // Ползунок
                float position = (float)((slider.Value - slider.Min) / (slider.Max - slider.Min));
                int handleX = (int)(slider.Bounds.X + (slider.Bounds.Width * position));
                graphics.FillRectangle(
                    SystemBrushes.ControlDark,
                    handleX - 2,
                    (int)slider.Bounds.Y,
                    4,
                    (int)slider.Bounds.Height
                );
            }

            // Зона для drag&drop
            var dropRect = new Rectangle(
                (int)_dropZone.X,
                (int)_dropZone.Y,
                (int)_dropZone.Width,
                (int)_dropZone.Height
            );

            graphics.DrawRectangle(
                SystemPens.ControlDark,
                dropRect
            );

            using (var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                graphics.DrawString(
                    "Перетащите сюда кривую сечения",
                    SystemFonts.DefaultFont,
                    SystemBrushes.ControlText,
                    dropRect,
                    format
                );
            }
        }

        public float GetRequiredHeight()
        {
            return (_sliders.Count * (SLIDER_HEIGHT + SLIDER_SPACING)) +
                   DROP_ZONE_HEIGHT + (2 * PADDING);
        }

        public bool ProcessMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e == null) return false;

            for (int i = 0; i < _sliders.Count; i++)
            {
                if (_sliders[i].Bounds.Contains(e.CanvasLocation))
                {
                    // Здесь будет реализация редактирования значения
                    _component.ExpireSolution(true);
                    return true;
                }
            }
            return false;
        }
    }
}
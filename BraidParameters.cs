using System;
using Rhino.Geometry;

namespace _3D_Braid
{
    public class BraidParameters
    {
        // Константы с граничными значениями
        public const double WIDTH_MIN = 1.0;
        public const double WIDTH_MAX = 30.0;
        public const double HEIGHT_MIN = 0.5;
        public const double HEIGHT_MAX = 6.0;
        public const double STEEPNESS_MIN = 0.1;
        public const double STEEPNESS_MAX = 4.0;
        public const int POINTS_MIN = 5;
        public const int POINTS_MAX = 60;
        public const double DIAMETER_MIN = 6.0;
        public const double DIAMETER_MAX = 30.0;
        public const double OFFSET_MIN = 0.0;
        public const double OFFSET_MAX = 2.0;
        public const int PERIODS_MIN = 2;
        public const int PERIODS_MAX = 30;

        // Значения по умолчанию
        private double _width = 6.0;
        private double _height = 2.0;
        private double _steepness = 1.0;
        private int _pointsPeriod = 20;
        private double _diameter = 18.0;
        private double _diameterOffset = 0.2;
        private int _numPeriods = 14;
        private Curve _sectionCurve;

        // Свойства с проверкой диапазонов
        public double Width
        {
            get { return _width; }
            set
            {
                if (value < WIDTH_MIN || value > WIDTH_MAX)
                    throw new ArgumentOutOfRangeException($"Width должен быть между {WIDTH_MIN} и {WIDTH_MAX} мм");
                _width = value;
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                if (value < HEIGHT_MIN || value > HEIGHT_MAX)
                    throw new ArgumentOutOfRangeException($"Height должен быть между {HEIGHT_MIN} и {HEIGHT_MAX} мм");
                _height = value;
            }
        }

        public double Steepness
        {
            get { return _steepness; }
            set
            {
                if (value < STEEPNESS_MIN || value > STEEPNESS_MAX)
                    throw new ArgumentOutOfRangeException($"Steepness должен быть между {STEEPNESS_MIN} и {STEEPNESS_MAX} мм");
                _steepness = value;
            }
        }

        public int PointsPeriod
        {
            get { return _pointsPeriod; }
            set
            {
                if (value < POINTS_MIN || value > POINTS_MAX)
                    throw new ArgumentOutOfRangeException($"Points/Period должен быть между {POINTS_MIN} и {POINTS_MAX}");
                _pointsPeriod = value;
            }
        }

        public double Diameter
        {
            get { return _diameter; }
            set
            {
                if (value < DIAMETER_MIN || value > DIAMETER_MAX)
                    throw new ArgumentOutOfRangeException($"Diameter должен быть между {DIAMETER_MIN} и {DIAMETER_MAX} мм");
                _diameter = value;
            }
        }

        public double DiameterOffset
        {
            get { return _diameterOffset; }
            set
            {
                if (value < OFFSET_MIN || value > OFFSET_MAX)
                    throw new ArgumentOutOfRangeException($"Diameter Offset должен быть между {OFFSET_MIN} и {OFFSET_MAX} мм");
                _diameterOffset = value;
            }
        }

        public int NumPeriods
        {
            get { return _numPeriods; }
            set
            {
                if (value < PERIODS_MIN || value > PERIODS_MAX)
                    throw new ArgumentOutOfRangeException($"Num Periods должен быть между {PERIODS_MIN} и {PERIODS_MAX}");
                _numPeriods = value;
            }
        }

        public Curve SectionCurve
        {
            get { return _sectionCurve; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Section Curve не может быть null");
                _sectionCurve = value;
            }
        }

        // Метод для проверки всех параметров сразу
        public bool ValidateAll(out string errorMessage)
        {
            try
            {
                // Проверяем каждый параметр
                Width = _width;
                Height = _height;
                Steepness = _steepness;
                PointsPeriod = _pointsPeriod;
                Diameter = _diameter;
                DiameterOffset = _diameterOffset;
                NumPeriods = _numPeriods;

                if (_sectionCurve == null)
                {
                    errorMessage = "Требуется секционная кривая";
                    return false;
                }

                errorMessage = string.Empty;
                return true;
            }
            catch (ArgumentException ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}
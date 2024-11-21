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
        public const double DIAMETER_MIN = 6.0;
        public const double DIAMETER_MAX = 30.0;
        public const double OFFSET_MIN = 0.0;
        public const double OFFSET_MAX = 4.0;
        public const int PERIODS_MIN = 2;
        public const int PERIODS_MAX = 30;
        public const double MIN_DIAMETER_MIN = 0.3;
        public const double MIN_DIAMETER_MAX = 3.0;
        public const double MAX_DIAMETER_MIN = 0.3;
        public const double MAX_DIAMETER_MAX = 3.0;
        public const double ROTATION_ANGLE_MIN = 0.0;
        public const double ROTATION_ANGLE_MAX = 90.0;

        // Значения по умолчанию
        private double _width = 6.0;
        private double _height = 2.0;
        private double _diameter = 18.0;
        private double _diameterOffset = 0.2;
        private int _numPeriods = 14;
        private double _minDiameter = 1.4;
        private double _maxDiameter = 2.0;
        private double _rotationAngle = 56.0;

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

        public double MinDiameter
        {
            get { return _minDiameter; }
            set
            {
                if (value < MIN_DIAMETER_MIN || value > MIN_DIAMETER_MAX)
                    throw new ArgumentOutOfRangeException($"Min Diameter должен быть между {MIN_DIAMETER_MIN} и {MIN_DIAMETER_MAX} мм");
                _minDiameter = value;
            }
        }

        public double MaxDiameter
        {
            get { return _maxDiameter; }
            set
            {
                if (value < MAX_DIAMETER_MIN || value > MAX_DIAMETER_MAX)
                    throw new ArgumentOutOfRangeException($"Max Diameter должен быть между {MAX_DIAMETER_MIN} и {MAX_DIAMETER_MAX} мм");
                _maxDiameter = value;
            }
        }

        public double RotationAngle
        {
            get { return _rotationAngle; }
            set
            {
                if (value < ROTATION_ANGLE_MIN || value > ROTATION_ANGLE_MAX)
                    throw new ArgumentOutOfRangeException($"Rotation Angle должен быть между {ROTATION_ANGLE_MIN} и {ROTATION_ANGLE_MAX} градусов");
                _rotationAngle = value;
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
                Diameter = _diameter;
                DiameterOffset = _diameterOffset;
                NumPeriods = _numPeriods;
                MinDiameter = _minDiameter;
                MaxDiameter = _maxDiameter;
                RotationAngle = _rotationAngle;                

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
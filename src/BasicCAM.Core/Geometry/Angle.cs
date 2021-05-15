using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAM.Core.Geometry
{
    public struct Angle
    {
        public double Value { get; }
        public bool IsRad { get; }
        public Angle(double value, bool isRad = true)
        {
            Value = value;
            IsRad = isRad;
        }

        public double Rad
        {
            get
            {
                if (IsRad)
                    return Value;

                return Value * 180.0 /Math.PI ;
            } 
        }
        public double Deg
        {
            get
            {
                if (IsRad)
                    return Value * Math.PI / 180.0;

                return Value;
            }
        }

        public static double Deg2Rad(double deg)
        {
            return deg * Math.PI / 180.0;
        }
    }
}

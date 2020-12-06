using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAM.Geometry
{
    public static class Angle
    {
        public static double Deg2Rad(double deg)
        {
            return deg * Math.PI / 180.0;
        }
    }
}

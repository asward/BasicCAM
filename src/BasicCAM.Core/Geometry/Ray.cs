using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAM.Geometry
{
    public class Ray : Line
    {
        public Point Start { get; set; }
        public Point ThroughPoint { get; set; }

        private double AngleRad { get; set; }
        public Ray(Point start, double angleRad) :base(start, angleRad)
        {
            Start = start;
            AngleRad = angleRad;
        }
    }
}

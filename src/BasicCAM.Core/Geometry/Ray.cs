using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAM.Core.Geometry
{
    public class Ray 
    {
        public Point Start { get; }
        public Angle Angle { get; }
        public Ray(Point start, double angleRad) 
        {
            Start = start;
            Angle = new Angle(angleRad);
        }

        public Ray(Point start, Point through)
        {
            Start = start;
            var v = new Vector(start, through);
            Angle= new Angle(v.AngleX);
        }
    }
}

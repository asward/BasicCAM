using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAM.Core.Geometry
{
    [Serializable]
    public struct Point
    {
        public static Point operator +(Point p, Vector v) => new Point(v.X + p.X, v.Y + p.Y, v.Z + p.Z);
        public static Point operator +(Vector v,Point p) => new Point(v.X + p.X, v.Y + p.Y, v.Z + p.Z);
        public static Point operator -(Point p, Vector v) => new Point(p.X - v.X, p.Y - v.Y, p.Z - v.Z);
        public static Vector operator -(Point p1, Point p2) => new Vector(p1, p2);

        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public Point(double x=0, double y=0, double z = 0)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public bool PracticallyEquals(Point p2)
        {
            //=> X == p2.X && Y == p2.Y && Z == p2.Z;
            return X.ToString("G10") == p2.X.ToString("G10")
            && Y.ToString("G10") == p2.Y.ToString("G10")
            && Z.ToString("G10") == p2.Z.ToString("G10");
        } 
        
    }
    public static class PointExtensions
    {
        public static Point ScaleFromOrigin(this Point p, double scale)
        {
            return new Point(p.X * scale, p.Y * scale, p.Z * scale);
        }
        public static bool Near(this Point p1, Point p2, double maxDist)
        {
            return p1.To(p2).Magnitude < maxDist;
        }

    }
}

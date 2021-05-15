using System;

namespace BasicCAM.Core.Geometry
{
    public class Vector
    {
        public static Vector operator +(Vector v1, Vector v2) => new Vector(v1.X+v2.X,v1.Y+v2.Y,v1.Z+v2.Z);
        public static Vector operator -(Vector v1, Vector v2) => new Vector(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);
        public static double operator *(Vector v1, Vector v2) => v2.X * v1.X+ v2.Y * v1.Y+ v2.Z * v1.Z;
        public static Vector operator *(double d, Vector v2) => new Vector(v2.X *d, v2.Y * d, v2.Z *d);
        public static Vector operator *(Vector v2,double d) => new Vector(v2.X * d, v2.Y * d, v2.Z * d);


        public double X;
        public double Y;
        public double Z;
        public Vector(double angleRad)
        {
            X = Math.Cos(angleRad);
            Y = Math.Sin(angleRad);
            Z = 0;
        }
        public Vector(Angle angle)
        {
            X = Math.Cos(angle.Rad);
            Y = Math.Sin(angle.Rad);
            Z = 0;
        }
        public Vector(Point end)
        {
            this.X = end.X;
            this.Y = end.Y;
            this.Z = end.Z;
        }
        public Vector(Point start, Point end)
        {
            this.X = end.X - start.X;
            this.Y = end.Y - start.Y;
            this.Z = end.Z - start.Z;
        }
        public Vector(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public double Magnitude
        {
            get
            {
                return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
            }
        }
        public double AngleX
        {
            get
            {
                return Math.Atan2(Y,X);
            }
        }
        public double AngleY
        {
            get
            {
                return Math.Atan2(X, Y);
            }
        }
        public double AngleZ
        {
            get
            {
                return Math.Atan2(X, Z);
            }
        }
        public double[] ToArray()
        {
            return new double[] { X, Y, Z };
        }
    }
    public static class VectorExtensions
    {
        public static Vector Cross(this Vector v1, Vector v2)
        {
            var i = v1.Y * v2.Z - v1.Z * v2.Y;
            var j = v1.X * v2.Z - v1.Z * v2.X;
            var k = v1.X * v2.Y - v1.Y * v2.X;

            return new Vector(i, j, k);
        }
        public static double AngleBetween(this Vector v1, Vector v2)
        {
            return  Math.Acos(v1*v2/ (v1.Magnitude * v2.Magnitude));
        }
        public static Vector To(this Point p1, Point p2)
        {
            return new Vector(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z); ;
        }
        public static  Vector Normalize( this Vector v)
        {
            var mag = v.Magnitude;

            if (0 == mag)
                return  v;

            v.X = v.X / mag;
            v.Y = v.Y / mag;
            v.Z = v.Z / mag;

            return  v;
        }
        public static  Vector Scale( this Vector v,double scale)
        {
            v.X = v.X * scale;
            v.Y = v.Y * scale;
            v.Z = v.Z * scale;

            return  v;
        }
        public static  Vector Rotate( this Vector v,double angle, PLANE plane = PLANE.XY)
        {
            double a = 0;
            double b = 0;
            switch (plane)
            {
                case (PLANE.XY):
                    a = v.X;
                    b = v.Y;
                    v.X = a * Math.Cos(angle) - b * Math.Sin(angle);
                    v.Y = a * Math.Sin(angle) + b * Math.Cos(angle);
                    break;

                case (PLANE.YZ):
                    a = v.Y;
                    b = v.Z;
                    v.Y = a * Math.Cos(angle) - b * Math.Sin(angle);
                    v.Z = a * Math.Sin(angle) + b * Math.Cos(angle);
                    break;
                case (PLANE.ZX):
                    a = v.Z;
                    b = v.X;
                    v.Z = a * Math.Cos(angle) - b * Math.Sin(angle);
                    v.X = a * Math.Sin(angle) + b * Math.Cos(angle);
                    break;
            }

            return  v;
        }
    }
}

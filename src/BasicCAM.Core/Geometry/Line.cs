using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAM.Geometry
{
    [Serializable]
    public class Line
    {
        private Point A { get; set; }
        private Point B { get; set; }
        //public double m { get; set; }
        //public double b { get; set; }
        public Line(Point p1, Point p2)
        {
            A = p1;
            B = p2;
        }
        public Line(Point p1, double angleRad)
        {
            A = p1;
            B = p1 + new Vector(angleRad);    
        }
        public Line PerpendicularAt(Point p)
        {
            return new Line(p, p + UnitVector.Rotate(Math.PI/2));
        }
        public Vector UnitVector { get
            {
                return A.To(B).Normalize();
            } 
        }
        public static Point Intersection(Line l1, Line l2)
        {
            var v1 = new Vector(l1.A, l1.B);
            var v2 = new Vector(l2.A, l2.B);

            var A = Matrix<double>.Build.DenseOfArray(new double[,] {
                { v1.X, v2.X },
                { v1.Y, v2.Y },
                { v1.Z, v2.Z }
            });
            var b = Vector<double>.Build.Dense(new double[] { l2.A.X - l1.A.X, l2.A.Y - l1.A.Y, l2.A.Z - l1.A.Z });

            var x = A.Solve(b); //Todo could be parallel

            return l1.A + x[0] * v1;
        }
        public Point NearestPoint(Point fromPoint)
        {
            var pLine = PerpendicularAt(fromPoint);

            return Line.Intersection(this, pLine);
        }
    }
}

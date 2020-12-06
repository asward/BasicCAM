using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicCAM.Geometry;
using BasicCAM.Preferences;
using BasicCAM.GCode;
using MathNet.Numerics.LinearAlgebra;

namespace BasicCAM.Segments
{
    [Serializable]
    public abstract class Segment
    {
        public virtual string Type { get; set; } = "Segment";

        public SegmentCAMSettings Settings { get; set; }

        public Point Start { get; set; }
        public Point End { get; set; }

        public virtual double XMax { get; }
        public virtual double YMax { get; }
        public virtual double XMin { get; }
        public virtual double YMin { get; }
        public virtual double Length { get; }
        public abstract double StartAngle { get; }
        public abstract double EndAngle { get; }

        public abstract double ApproxWinding();
        
        public abstract Segment Clone();
        public abstract Segment Shift(double x, double y, double z);
        public abstract Segment Offset(double offset);
        public virtual Segment Reverse()
        {
            var temp = this.Start;
            this.Start = this.End;
            this.End = temp;
            return this;
        }
    
        public static Point Intersection(Segment s1, Segment s2)
        {
            switch (s1)
            {
                case LinearSegment l1:
                    switch (s2)
                    {
                        case LinearSegment l2:
                            return Segment.Intersection(l1, l2);
                        case ArcSegment a2:
                            return Segment.Intersection(l1, a2);
                    }
                    break;
                case ArcSegment a1:
                    switch (s2)
                    {
                        case LinearSegment l2:
                            return Segment.Intersection(a1, l2);
                        case ArcSegment a2:
                            return Segment.Intersection(a1, a2);
                    }
                    break;
            }
            return null;
        }
        //private static Point Intersection(ArcSegment a1, LinearSegment l2)
        //{
        //    //Newtons method for intersection solutions
        //    var p = a1.Start; //make a guess
        //    while(SolutionError(p) > 0.001)
        //    {
        //        LinearSegment l1 = LinearSegment.BuildLinearSegment(a1.Start, a1.End,1).First();

        //        var p_line = Segment.Intersection(l1, l2);

        //        if (null == p_line)
        //            return null; //No Solution

        //        //Project to arc as final solution

        //    }

        //    return p;

        //    double SolutionError(Point p)
        //    {
        //        return 0;
        //    }
        //}
        private static Point Intersection(LinearSegment l1, ArcSegment a2)
        {
            return Segment.Intersection(a2, l1);
        }

        private static Point Intersection(ArcSegment a1, LinearSegment l2)
        {
            //Secant method
            //Find secant

            //Find nearest point to line
            var nearestPoint = l2.AsLine().NearestPoint(a1.Center);
            var lineVec = new Vector(l2.Start, l2.End);
            var vecToNearest = nearestPoint - a1.Center;

            if (vecToNearest.Magnitude > a1.Radius)
                return null; //no intersect

            //Half secent length = Sqrt( NearestPointDist^2 + Radius^2 )
            var halfSec = Math.Sqrt(Math.Pow(a1.Radius, 2) - Math.Pow(vecToNearest.Magnitude, 2));


            //Find intersection points
            var i1 = nearestPoint + lineVec.Normalize().Scale(halfSec);
            var i2 = nearestPoint + lineVec.Normalize().Scale(-halfSec);

            //Find one point which is on both elements
            //Search line first, more likely only one lands there
            if (onLinearSegment(i1) && onArcSegment(i1))
                return i1;

            if (onLinearSegment(i2) && onArcSegment(i2))
                return i2;

            bool onLinearSegment(Point p){
                //Point is on segment if 
                //vector from start to point to end is less than from start to point
                //AND
                //vector from end to point to start is less than from end to point

                var vse = new Vector(l2.Start, l2.End);
                var ves = new Vector(l2.End, l2.Start);

                var vsp = new Vector(l2.Start, p);
                var vep = new Vector(l2.End, p);

                if(vse.Magnitude > vsp.Magnitude && ves.Magnitude > vep.Magnitude)
                    return true;

                return false;
            }
            bool onArcSegment(Point p)
            {
                //Point is on arc if c->p angle is within c->start and c->end angle bounds
                //Check Arc
                var ase = new Vector(a1.Center, a1.Start).AngleBetween(new Vector(a1.Center, a1.End));
                var asp = new Vector(a1.Center, a1.Start).AngleBetween(new Vector(a1.Center, p));

                if (Math.Sign(ase) == Math.Sign(asp) && ase > asp)
                    return true;

                return false;
            } 

            return null;
        }
        //private static Point Intersection(ArcSegment a1, LinearSegment l2)
        //{
        //    var dx = l2.End.X - l2.Start.X;
        //    var dy = l2.End.Y - l2.Start.Y;

        //    var dr = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        //    var d = l2.Start.X * l2.End.Y - l2.End.X * l2.Start.Y ;


        //    var sqrtVal = Math.Pow(a1.Radius, 2) * Math.Pow(dr, 2) - Math.Pow(d, 2);

        //    if (sqrtVal < 0) //no intersections
        //        return null;


        //    var xp = (d * dy + Math.Sign(dy) * dx * Math.Sqrt(sqrtVal)) / Math.Pow(dr, 2);
        //    var xm = (d * dy - Math.Sign(dy) * dx * Math.Sqrt(sqrtVal)) / Math.Pow(dr, 2);

        //    var yp = (-d * dx + Math.Abs(dy) * Math.Sqrt(sqrtVal)) / Math.Pow(dr, 2);
        //    var ym = (-d * dx - Math.Abs(dy) * Math.Sqrt(sqrtVal)) / Math.Pow(dr, 2);

        //    return null;
        //}
        private static Point Intersection(ArcSegment a1, ArcSegment a2)
        {
            return null;
        }

        private static Point Intersection(LinearSegment l1, LinearSegment l2)
        {
            var v1 = new Vector(l1.Start, l1.End);
            var v2 = new Vector(l2.Start, l2.End);

            var A = Matrix<double>.Build.DenseOfArray(new double[,] {
                { v1.X, v2.X },
                { v1.Y, v2.Y },
                { v1.Z, v2.Z }
            });
            var b = Vector<double>.Build.Dense(new double[] { l2.Start.X - l1.Start.X, l2.Start.Y - l1.Start.Y, l2.Start.Z - l1.Start.Z });

            var x = A.Solve(b);

            if (0 <= x[0] && x[0] <= 1)//Intersection was within our segment
                return l1.Start + x[0] * v1;

            return null;
        }
    }

    public static class SegmentExtensions
    {
        public static List<Segment> ReverseSegements(this List<Segment> input)
        {
            var reversedList = input.Select(item => (Segment)item.Clone().Reverse()).ToList();

            reversedList.Reverse();

            return reversedList;
        }
        public static List<Segment> ShiftSegments(this List<Segment> input,double x=0,double y=0,double z =0)
        {
            return input.Select(item => (Segment)item.Shift(x,y,z)).ToList();
        }
        public static List<Segment> Clone(this List<Segment> input)
        {
            return input.Select(item => (Segment)item.Clone()).ToList();
        }
        public static List<Segment> SetSettings(this List<Segment> input, SegmentCAMSettings settings)
        {
             input.ForEach(item => item.Settings = settings);
            return input;
        }
    }
}

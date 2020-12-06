using System;
using System.Collections.Generic;
using System.Text;
using BasicCAM.Geometry;
using BasicCAM.GCode;
using MathNet.Numerics.LinearAlgebra;

namespace BasicCAM.Segments
{
    [Serializable]
    public class LinearSegment : Segment
    {

        public override string Type { get; set; } = "Linear";
      
      
        public override double XMax
        {
            get
            {
                return Math.Max(End.X, Start.X);
            }
        }

        public override double YMax
        {
            get
            {
                return Math.Max(End.Y, Start.Y);
            }
        }
        public override double XMin
        {
            get
            {
                return Math.Min(End.X, Start.X);
            }
        }

        public override double YMin
        {
            get
            {
                return Math.Min(End.Y, Start.Y);
            }
        }

        private double? _length = null;
        public override double Length
        {
            get 
            {
                if(null == _length)
                {
                    _length = Math.Sqrt(Math.Pow(End.X - Start.X, 2) + Math.Pow(End.Y - Start.Y, 2)); 
                }
                return (double) _length;
            }
        }

        /// <summary>
        /// + Offset is left of segment, - is right
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public override Segment Offset(double offset)
        {
            //Offset with + value are left of direction
            //Offset with - value are right of direction
            var newSegment = new LinearSegment();
            var v = new Vector(Start, End);

             v
                .Normalize()
                .Scale(offset)
                .Rotate(Math.PI / 2);

            Start = Start + v;
            End = End + v;
            return this;
        }


        public override Segment Clone()
        {
            return new LinearSegment()
            {
                Start = Start,
                End = End,
                Settings = Settings
            };
        }
        public override Segment Shift(double x, double y, double z)
        {
            Start = new Point(Start.X + x, Start.Y + y, Start.Z + z);
            End = new Point(End.X + x, End.Y + y, End.Z + z);

            return this;
        }

        public override double ApproxWinding()
        {
            return (End.X - Start.X) * (End.Y + Start.Y);
        }

        //private Point Intersection(LinearSegment l)
        //{
        //    var v1 = new Vector(Start, End);
        //    var v2 = new Vector(l.Start, l.End);

        //    var A = Matrix<double>.Build.DenseOfArray(new double[,] {
        //        { v1.X, v2.X },
        //        { v1.Y, v2.Y },
        //        { v1.Z, v2.Z }
        //    });
        //    var b = Vector<double>.Build.Dense(new double[] { l.Start.X-Start.X,l.Start.Y-Start.Y,l.Start.Z-Start.Z});

        //    var x = A.Solve(b);

        //    if (0 <= x[0] && x[0] <= 1)//Intersection was within our segment
        //        return Start + x[0] * v1;


        //    //var M = Matrix<double>.Build;


        //    //var m_num = M.Dense(2, 2);

        //    //m_num[0, 0] = l.Start.X - Start.X;
        //    //m_num[1, 0] = l.Start.Y - Start.Y;
        //    //m_num[0, 1] = v2.X;
        //    //m_num[1, 1] = v2.Y;



        //    //var m_den = M.Dense(2, 2);

        //    //m_den[0, 0] = v1.X;
        //    //m_den[1, 0] = v1.Y;
        //    //m_den[0, 1] = v2.X;
        //    //m_den[1, 1] = v2.Y;

        //    //var den = m_den.L2Norm();
        //    //var num = m_num.L2Norm();

        //    //if (den == 0)
        //    //{
        //    //    //something else
        //    //}


        //    //var t = num / den;
        //    //if(0<=t && t<=1)//Intersection was within our segment
        //    //    return Start + t * v1; 

        //    return null;
        //}

        public override double StartAngle
        {
            get
            {
                var v = new Vector(Start, End);
                return v.AngleX;
            }
        }

        public override double EndAngle
        {
            get
            {
                var v = new Vector(Start, End);
                return v.AngleX;
            }
        }

        public double MinDistance(LinearSegment segment)
        {
            var vl1 = new Vector(Start, End);
            var vl2 = new Vector(segment.Start, segment.End);

            var normal = Vector.Cross(vl1, vl2);

            var d1 = -(normal.X * Start.X + normal.Y * Start.Y + normal.Z * Start.Z);
            var d2 = -(normal.X * segment.Start.X + normal.Y * segment.Start.Y + normal.Z * segment.Start.Z);

            return Math.Abs(d1 - d2) / (normal.Magnitude);
        }
        public double MinDistance(ArcSegment segment)
        {
            var vl1 = new Vector(Start, End);
            var vl2 = new Vector(segment.Start, segment.End);

            var normal = Vector.Cross(vl1, vl2);

            var d1 = -(normal.X * Start.X + normal.Y * Start.Y + normal.Z * Start.Z);
            var d2 = -(normal.X * segment.Start.X + normal.Y * segment.Start.Y + normal.Z * segment.Start.Z);

            return Math.Abs(d1 - d2) / (normal.Magnitude);
        }


        public Line AsLine()
        {
            return new Line(Start, End);
        }
        //public Point ShortestDistancePoint(LinearSegment l)
        //{

        //    v1.L2Norm();
        //    //def closestDistanceBetweenLines(a0, a1, b0, b1, clampAll= False, clampA0= False, clampA1= False, clampB0= False, clampB1= False):

        //    //    ''' Given two lines defined by numpy.array pairs (a0,a1,b0,b1)
        //    //        Return the closest points on each segment and their distance
        //    //    '''

        //    //    # If clampAll=True, set all clamps to True
        //    //            if clampAll:
        //    //        clampA0 = True
        //    //        clampA1 = True
        //    //        clampB0 = True
        //    //        clampB1 = True


        //    //    # Calculate denomitator
        //    //            A = a1 - a0
        //    //    B = b1 - b0
        //    //    magA = np.linalg.norm(A)
        //    //    magB = np.linalg.norm(B)

        //    //    _A = A / magA
        //    //    _B = B / magB

        //    //    cross = np.cross(_A, _B);
        //    //            denom = np.linalg.norm(cross) * *2


        //    //    # If lines are parallel (denom=0) test if lines overlap.
        //    //# If they don't overlap then there is a closest point solution.
        //    //# If they do overlap, there are infinite closest positions, but there is a closest distance
        //    //            if not denom:
        //    //                d0 = np.dot(_A, (b0 - a0))

        //    //        # Overlap only possible with clamping
        //    //            if clampA0 or clampA1 or clampB0 or clampB1:
        //    //            d1 = np.dot(_A, (b1 - a0))

        //    //            # Is segment B before A?
        //    //            if d0 <= 0 >= d1:
        //    //                if clampA0 and clampB1:
        //    //                    if np.absolute(d0) < np.absolute(d1):
        //    //                        return a0,b0,np.linalg.norm(a0 - b0)
        //    //                    return a0,b1,np.linalg.norm(a0 - b1)


        //    //            # Is segment B after A?
        //    //            elif d0 >= magA <= d1:
        //    //                if clampA1 and clampB0:
        //    //                    if np.absolute(d0) < np.absolute(d1):
        //    //                        return a1,b0,np.linalg.norm(a1 - b0)
        //    //                    return a1,b1,np.linalg.norm(a1 - b1)


        //    //        # Segments overlap, return distance between parallel segments
        //    //            return None,None,np.linalg.norm(((d0 * _A) + a0) - b0)



        //    //    # Lines criss-cross: Calculate the projected closest points
        //    //            t = (b0 - a0);
        //    //            detA = np.linalg.det([t, _B, cross])
        //    //    detB = np.linalg.det([t, _A, cross])

        //    //    t0 = detA / denom;
        //    //            t1 = detB / denom;

        //    //            pA = a0 + (_A * t0) # Projected closest point on segment A
        //    //    pB = b0 + (_B * t1) # Projected closest point on segment B


        //    //    # Clamp projections
        //    //            if clampA0 or clampA1 or clampB0 or clampB1:
        //    //        if clampA0 and t0< 0:
        //    //            pA = a0
        //    //        elif clampA1 and t0 > magA:
        //    //            pA = a1

        //    //        if clampB0 and t1< 0:
        //    //            pB = b0
        //    //        elif clampB1 and t1 > magB:
        //    //            pB = b1

        //    //        # Clamp projection A
        //    //            if (clampA0 and t0< 0) or(clampA1 and t0 > magA):
        //    //            dot = np.dot(_B, (pA - b0))
        //    //            if clampB0 and dot< 0:
        //    //                dot = 0
        //    //            elif clampB1 and dot > magB:
        //    //                dot = magB
        //    //            pB = b0 + (_B * dot)

        //    //        # Clamp projection B
        //    //            if (clampB0 and t1< 0) or(clampB1 and t1 > magB):
        //    //            dot = np.dot(_A, (pB - a0))
        //    //            if clampA0 and dot< 0:
        //    //                dot = 0
        //    //            elif clampA1 and dot > magA:
        //    //                dot = magA
        //    //            pA = a0 + (_A * dot)


        //    //    return pA,pB,np.linalg.norm(pA - pB)
        //}

        public static IEnumerable<LinearSegment> BuildLinearSegment(Point p1, Point p2, double scale = 1)
        {
            return new List<LinearSegment>()
            {
                { new LinearSegment()
                {
                    Start = p1.ScaleFromOrigin(scale),
                    End = p2.ScaleFromOrigin(scale),
                } }
            };
        }
    }
}

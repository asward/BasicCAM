using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicCAM.Core.Geometry;
using MathNet.Numerics.LinearAlgebra;

namespace BasicCAM.Core.Segments
{
    public static class Intersections
    {

        public static bool TryIntersection(Segment s1, Segment s2, out Point p)
        {
            p = new Point();
            switch (s1)
            {
                case LinearSegment l1:
                    switch (s2)
                    {
                        case LinearSegment l2:
                            return l1.TryIntersection(l2, ref p);
                        case ArcSegment a2:
                            return l1.TryIntersection(a2, ref p);
                    }
                    break;
                case ArcSegment a1:
                    switch (s2)
                    {
                        case LinearSegment l2:
                            return a1.TryIntersection(l2, ref p);
                        case ArcSegment a2:
                            return a1.TryIntersection(a2, ref p);
                    }
                    break;
            }
            p = new Point();
            return false;
        }

        public static bool TryIntersection(this LinearSegment l1, ArcSegment a2, ref Point p)
        {
            return a2.TryIntersection(l1, ref p);
        }

        public static bool TryIntersection(this ArcSegment a1, LinearSegment l2, ref Point p)
        {
            //Secant method
            //Find secant

            //Find nearest point to line
            var lineL2 = new Line(l2.Start, l2.End);
            var nearestPoint = lineL2.NearestPoint(a1.Center);
            var lineVec = new Vector(l2.Start, l2.End);
            var vecToNearest = nearestPoint - a1.Center;

            if (vecToNearest.Magnitude > a1.Radius)
                return false; //no intersect

            //Half secent length = Sqrt( NearestPointDist^2 + Radius^2 )
            var halfSec = Math.Sqrt(Math.Pow(a1.Radius, 2) - Math.Pow(vecToNearest.Magnitude, 2));


            //Find intersection points
            var i1 = nearestPoint + lineVec.Normalize().Scale(halfSec);
            var i2 = nearestPoint + lineVec.Normalize().Scale(-halfSec);

            //Find one point which is on both elements
            //Search line first, more likely only one lands there
            if (l2.IsOnSegment(i1) && a1.IsOnSegment(i1)) //onLinearSegment(i1) && onArcSegment(i1)
            {
                p = i1;
                return true;
            }
                

            if (l2.IsOnSegment(i2) && a1.IsOnSegment(i2)) //onLinearSegment(i2) &&onArcSegment(i2)
            {
                p = i2;
                return true;
            }


            return false;
        }

        public static bool TryIntersection(this ArcSegment a1, ArcSegment a2, ref Point p)
        {
            
            //NO points - Centers further than intersection would allow
            if((a1.Center - a2.Center).Magnitude > a1.Radius + a2.Radius)
                return false;

            var va12 = new Vector(a1.Center, a2.Center);


            //ONE point
            if (va12.Magnitude == a1.Radius + a2.Radius)
            {
                var ponly = a1.Center + va12.Normalize().Scale(a1.Radius);
                if (a1.IsOnSegment(ponly) && a2.IsOnSegment(ponly))
                {
                    p = ponly;
                    return true;
                }
                return false;
            }

            //TWO points
            //Pick first of the two and return.
            //d = distance center to center
            var d = va12.Magnitude;
            //x = distance from center for a1 to 'lens' centerline
            var x = (Math.Pow(d, 2) - Math.Pow(a2.Radius, 2) + Math.Pow(a1.Radius, 2)) / (2 * d);
            //theta = angle from va12 to intersection point on a1
            var theta = Math.Acos(x / a1.Radius);

            var p1 = a1.Center + va12.Normalize().Scale(a1.Radius).Rotate(theta);
            var p2 = a1.Center + va12.Normalize().Scale(a1.Radius).Rotate(-theta);

            if(a1.IsOnSegment(p1) && a2.IsOnSegment(p1))
            {
                p = p1;
                return true;
            }

            if (a1.IsOnSegment(p2) && a2.IsOnSegment(p2))
            {
                p = p2;
                return true;
            }

            return false;
        }

        public static bool TryIntersection(this LinearSegment l1, LinearSegment l2, ref Point p)
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
            {
                p = l1.Start + x[0] * v1;
                return true;
            }

            return false;
        }

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


}

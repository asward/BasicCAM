using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicCAM.Core.Geometry;
using BasicCAM.Core.Preferences;

namespace BasicCAM.Core.Segments
{
    public static class SegmentExtensions
    {
        public static double MinDistance(this LinearSegment fromSegment, LinearSegment toSegment)
        {
            var vl1 = new Vector(fromSegment.Start, fromSegment.End);
            var vl2 = new Vector(toSegment.Start, toSegment.End);

            var normal = vl1.Cross(vl2);

            var d1 = -(normal.X * fromSegment.Start.X + normal.Y * fromSegment.Start.Y + normal.Z * fromSegment.Start.Z);
            var d2 = -(normal.X * toSegment.Start.X + normal.Y * toSegment.Start.Y + normal.Z * toSegment.Start.Z);

            return Math.Abs(d1 - d2) / (normal.Magnitude);
        }
        public static double MinDistance(this LinearSegment fromSegment, ArcSegment toSegment)
        {
            var vl1 = new Vector(fromSegment.Start, fromSegment.End);
            var vl2 = new Vector(toSegment.Start, toSegment.End);

            var normal = vl1.Cross(vl2);

            var d1 = -(normal.X * fromSegment.Start.X + normal.Y * fromSegment.Start.Y + normal.Z * fromSegment.Start.Z);
            var d2 = -(normal.X * toSegment.Start.X + normal.Y * toSegment.Start.Y + normal.Z * toSegment.Start.Z);

            return Math.Abs(d1 - d2) / (normal.Magnitude);
        }
        public static void ReverseSegements(this List<Segment> input)
        {
            input.ForEach(s => s.Reverse());

            input.Reverse();
        }
        public static List<Segment> ShiftSegments(this List<Segment> input, Vector direction)
        {
            return input.Select(item => (Segment)item.Shift(direction.X, direction.Y, direction.Z)).ToList();
        }
        public static List<Segment> Clone(this IEnumerable<Segment> input)
        {
            return input.Select(item => (Segment)item.ShallowCopy()).ToList();
        }


        
        public static List<Segment> TrimOverlapingSegments(this List<Segment> segments)
        {
            //No interference if only a single segment
            if (segments.Count == 1)
                return segments;

            //Trim overlapped segments
            for (int i = segments.Count - 1; i >= 0; i--)
            {
                var behindIndex = (i - 1) < 0 ? segments.Count - 1 : i - 1;
                var aheadIndex = i;
                var b = segments[behindIndex];
                var a = segments[aheadIndex];

                //If segments don't intersect, pass to next
                if (!Intersections.TryIntersection(a, b, out Point p))
                    continue;

                b.SetEnd(p);
                a.SetStart(p);
            }

            return segments;
        }
        public static List<Segment> FillGaps(this List<Segment> segments)
        {
            //No gaps if only a single segment
            if (segments.Count == 1)
                return segments;

            //Else, insert arc to bridge the gap

            //Exted any open portions of the loop need tied with arcs
            //Loop backwards so we can insert along the way, without affecting the loop
            for (int i = segments.Count - 1; i >= 0; i--)
            {
                var behindIndex = (i - 1) < 0 ? segments.Count - 1 : i - 1;
                var aheadIndex = i;
                var b = segments[behindIndex];
                var a = segments[aheadIndex];

                if (a.Start.PracticallyEquals(b.End))
                    continue;

                var arc = ArcBetweenOffsetSegments(a, b);
                if (null != arc)
                {
                    segments.Insert(aheadIndex, arc);
                }

            }


            ArcSegment ArcBetweenOffsetSegments(Segment ahead, Segment behind)
            {
                var startAngle = new Vector(behind.EndAngle);
                var endAngle = new Vector(ahead.StartAngle);

                var theta = endAngle.AngleBetween(startAngle);
                if (Double.IsNaN(theta))
                    return null;

                var startV = new Vector(behind.EndAngle);
                var endV = new Vector(ahead.StartAngle);
                var cw = endV.Cross(startV).Z > 0;

                var b = Math.Abs(Math.Tan(theta / 4)) * (cw ? -1 : 1);

                var arc = new ArcSegment(behind.End, ahead.Start, b);
                if (Double.IsInfinity(arc.Radius))
                {
                    return null;
                }
                return arc;

            }

            return segments;
        }

        /// <summary>
        /// Creates and appends clones of Segments offset by zStepOver.
        /// </summary>
        /// <param name="segments"></param>
        /// <param name="n">Number of patterened copies (n>0)</param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static List<Segment> LinearPatternFeature(this List<Segment> segments, int n, Vector direction)
        {
            if (n <= 0)
                return segments;

            var previousPass = segments.Clone(); //Copy of segments to reclone and reverse as needed

            //Passes/stepdown - open loop reverse for second pass
            for (int pass = 1; pass < n; pass++) //First pass exists as provided segments list
            {
                var passSegments = previousPass.Clone();

                passSegments.ShiftSegments(direction); //Shift this pass down by offset for the pass

                segments.AddRange(passSegments);
                previousPass = passSegments;
            }

            return segments;
        }

        /// <summary>
        /// Provides offset required for feature using given tool diameter. Tool diameter always assumed positive.
        /// </summary>
        /// <param name="toolDiameter"></param>
        /// <returns></returns>
        public static double SignedOffset(this IEnumerable<Segment> segments, double distance, CutSide cutSide)
        {
            if (cutSide == CutSide.Center || !segments.IsClosedLoop())
                return 0;

            //A           B            X            A B X
            //CW with  OUTSIDE is - (LEFT) radius   1 1 0
            //CW with  INSIDE is + (RIGHT) radius   1 0 1
            //CCW with OUTSIDE is + (RIGHT)radius   0 1 1
            //CCW with INSIDE is - (LEFT) radius    0 0 0

            //A XNOR B  is same as A == B
            bool cutsideIsOutside = cutSide == CutSide.Outside;
            //         A                B
            return segments.IsCW() == cutsideIsOutside ? -distance : distance;
        }
        public static List<Segment> OffsetSegements(this List<Segment> segments, double distance, CutSide cutSide)
        {
            var signedDistance = SignedOffset(segments, distance, cutSide);    
            return segments.OffsetSegements(signedDistance);
        }
        /// <summary>
        /// Offsets segments by distance amount. Uses sign of distance to determine left/right of segment
        /// </summary>
        /// <param name="segments"></param>
        /// <param name="distance">Negative values offset left of direction, positive to right</param>
        /// <returns></returns>
        public static List<Segment> OffsetSegements(this List<Segment> segments, double distance)
        {
            // No change needed
            if (distance == 0)
                return segments;

            //Offsetting
            segments.ForEach(s => s.Offset(distance));

            return segments;
        }


        /// <summary>
        /// Returns true for segment list which is completelly closed (within provided tolernace).
        /// </summary>
        /// <param name="segments"></param>
        /// <returns></returns>
        public static bool IsClosedLoop(this IEnumerable<Segment> segments, double tolerance = 0.0001)
        {
            for (int i = 0; i < segments.Count() - 1; i++)
            {
                if (segments.ElementAt(i).End.To(segments.ElementAt(i+1).Start).Magnitude > tolerance)
                    return false;
            }

            if (segments.ElementAt(segments.Count() - 1).End.To(segments.ElementAt(0).Start).Magnitude > tolerance)
                return false;

            return true;
        }

        public static bool IsCW(this IEnumerable<Segment> segments)
        {
            double sum = 0;
            foreach (var segment in segments)
            {
                sum += segment.ApproxWinding;
            }

            return sum > 0.0 ? true : false;
        }

        public static double Length(this List<Segment> segments)
        {
            return segments.Sum(s => s.Length);
        }
        //public static List<ArcSegment> ArcsTangentTo(this Segment ahead, Segment behind, bool CW)
        //{
        //    var arcs = new List<ArcSegment>();

        //    //if points joining are the same, return empty list
        //    if (ahead.Start.Equals(behind.End))
        //        return arcs;

        //    var startAngle = new Vector(behind.EndAngle);
        //    var endAngle = new Vector(ahead.StartAngle);

        //    var theta = endAngle.AngleBetween(startAngle);
        //    var b = Math.Abs(Math.Tan(theta / 4)) * (CW ? -1 : 1);

        //    arcs = ArcSegment.New(behind.End, ahead.Start, b);

        //    return arcs;

        //}

    }
}

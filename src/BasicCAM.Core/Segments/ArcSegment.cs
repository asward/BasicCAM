using System;
using System.Collections.Generic;
using System.Text;
using BasicCAM.Geometry;
using BasicCAM.GCode;

namespace BasicCAM.Segments
{
    [Serializable]
    public class ArcSegment : Segment
    { public override string Type { get; set; } = "Arc";
     
        public Point Center { get; set; }
        public double Radius { get; set; }
        public bool CW { get; set; } = true;

        public override Segment Clone()
        {
            return new ArcSegment()
            {
                Type = Type,
                Start = Start,
                End = End,
                Center = Center,
                Radius = Radius,
                CW =CW,
                Settings = Settings
            };
        }

        /// <summary>
        /// + Offset is larger radius, - is smaller 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public override Segment Offset(double offset)
        {
            var startOffset = new Vector(Center, Start);
            var endOffset = new Vector(Center, End);

            startOffset.Normalize().Scale(offset);
            endOffset.Normalize().Scale(offset);

            Start += startOffset;
            End += endOffset;
            Radius += offset;
            return this;
        }

        public override double ApproxWinding()
        {
            var k = 5;
            List<Point> approxPoints = new List<Point>();
            for (int n = 0; n < k; n++)
            {
                Point p = new Point(Center.X + Radius * Math.Cos(t(n)), Center.Y + Radius * Math.Sin(t(n)));
                approxPoints.Add(p);
            }

            double sum = 0;
            for (int i = 0; i < approxPoints.Count-1; i++)
            {
                var p1 = approxPoints[i];
                var p2 = approxPoints[i + 1];
                sum += (p2.X - p1.X) * (p2.Y + p1.Y);
            }

            sum += (approxPoints[0].X - approxPoints[approxPoints.Count - 1].X) * (approxPoints[0].Y + approxPoints[approxPoints.Count - 1].Y);

            //Break arc into list of line segments
            return sum;

            double t(int n)
            {
                return k / (n + 1) * 2 * Math.PI;
            }
        }
       
        public override Segment Reverse()
        {
            base.Reverse();
            CW = !CW;

            return this;
        }
        public override Segment Shift(double x, double y, double z)
        {
            Start = new Point(Start.X + x, Start.Y + y, Start.Z + z);
            End = new Point(End.X + x, End.Y + y, End.Z + z);
            Center = new Point(Center.X + x, Center.Y + y, Center.Z + z);

            return this;
        }

        /// <summary>
        /// Returns quadrant releative to the center of the arc for the given point
        /// 1 - +x/+y
        /// 2 - -x/+y
        /// 3 - -x/-y
        /// 4 - +x/-y
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private int FindQuadrant(Point p)
        {
            var v = Center.To(p);

            if (v.X >= 0 && v.Y >0)
                return 1;
            if (v.X < 0 && v.Y >= 0)
                return 2;
            if (v.X <= 0 && v.Y < 0)
                return 3;
            return 4;
        }

        /// <summary>
        /// Lists which local axes the Arc crosses 
        /// 1 - +x
        /// 2 - +y
        /// 3 - -x
        /// 4 - -y
        /// </summary>
        /// <returns></returns>
        public List<int> AxesCrossed()
        {
            var axesCrossed = new List<int>();

            var startQuad = FindQuadrant(Start);
            var endQuad = FindQuadrant(End);

            var currentQuad = startQuad;

            while (endQuad != currentQuad)
            {
                if (CW)
                {
                    axesCrossed.Add(currentQuad);
                    currentQuad = currentQuad - 1;
                    if (currentQuad == 0)
                        currentQuad = 4;
                }
                else
                {
                    currentQuad = currentQuad + 1;
                    if (currentQuad == 5)
                        currentQuad = 1;
                    axesCrossed.Add(currentQuad);
                }
            }

            return axesCrossed;
        }

        /// <summary>
        /// Angle reletive to +x axis of a line from center to given p
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private double Angle(Point p) { 
                if (p.X - Center.X == 0)
                    return p.Y - Center.Y > 0 ? Math.PI / 2 : Math.PI/2+ Math.PI;
                
                return Math.Atan2((p.Y - Center.Y) , (p.X - Center.X)) ;
        }

        public override double StartAngle
        {
            get 
            {
                var value = CW ? Angle(Start) - Math.PI / 2 : Angle(Start) + Math.PI / 2;
                return value % (2 * Math.PI);
            }
        }

        public override double EndAngle
        {
            get
            {
                var value = CW ? Angle(End) - Math.PI / 2 : Angle(End) + Math.PI / 2;
                return value % (2 * Math.PI);
            }
        }

        public override double XMax
        {
            get
            {
                if (AxesCrossed().Contains(1))
                    return Center.X + Radius;
                return Math.Max(End.X, Start.X);
            }
        }
        public override double YMax
        {
            get
            {
                if (AxesCrossed().Contains(2))
                    return Center.Y + Radius;
                return Math.Max(End.Y, Start.Y);
            }
        }
        public override double XMin
        {
            get
            {
                if (AxesCrossed().Contains(3))
                    return Center.X - Radius;
                return Math.Min(End.X, Start.X);
            }
        }
        public override double YMin
        {
            get
            {
                if (AxesCrossed().Contains(4))
                    return Center.Y - Radius;
                return Math.Min(End.Y, Start.Y);
            }
        }

        private double? _length = null;
        public override double Length
        {
            get
            {
                if (null == _length)
                {
                    _length = Radius * Math.Abs(EndAngle-StartAngle);
                }
                return (double)_length;
            }
        }
        public static List<ArcSegment> BuildArcSegment(Point p1, Point p2, double buldge, double scale = 1)
        {
            var theta = 4 * Math.Atan(buldge);
            var d = p1.To(p2).Magnitude / 2;
            var s = buldge * d;
            var r = (Math.Pow(s, 2) + Math.Pow(d, 2)) / (2 * s);


            var alpha = (Math.PI / 2 - Math.Abs(theta / 2))*Math.Sign(buldge);
            //var alpha = sign * Math.Abs(buldge);
            var buldge_dist = d / Math.Tan(theta / 2);

            ////LINES METHOD
            ////CAN"T DETERMINE DIRECTION OF buldgeVector
            //Point midPoint = PointOperations.MidPoint(p1, p2);
            //Line connectingLine = new Line(p1, p2);
            //Line PerpConnectingLine = connectingLine.PerpendicularAt(midPoint);
            //Point buldgeVector = PerpConnectingLine.UnitVector.Scaled(-buldge_dist);
            //Point center = midPoint.Distance(buldgeVector);

            //ANGLES METHOD
            Vector vectorToCenter =
                p1
                .To(p2)
                .Normalize()
                .Scale(Math.Abs(r))
                .Rotate(alpha);

            Point center = p1 + vectorToCenter ;

            return ArcSegment.BuildArcSegment(center.ScaleFromOrigin(scale), Math.Abs(r * scale), p1.ScaleFromOrigin(scale), p2.ScaleFromOrigin(scale), buldge < 0);
        }
        public static List<ArcSegment> BuildArcSegment(Point center, double radius, Point start_point, Point end_point, bool cw = true, double scale = 1)
        {
            return new List<ArcSegment>()
                {

                    //TODO SPLIT OUT THE ARCS?
                    new ArcSegment()
                    {
                        Radius = radius*scale,
                        Center = center.ScaleFromOrigin(scale),
                        Start = start_point.ScaleFromOrigin(scale),
                        End = end_point.ScaleFromOrigin(scale),
                        CW = cw
                    }
                };
        }
        public static List<ArcSegment> BuildArcSegment(Point center, double radius, double start_angle, double end_angle, double scale = 1)
        {
            return new List<ArcSegment>()
            {

                //TODO SPLIT OUT THE ARCS?
                new ArcSegment()
                {
                    Radius = radius*scale,
                    Center = center.ScaleFromOrigin(scale),
                    Start = (new Point(
                                  Math.Cos(start_angle) * radius + center.X,
                                  Math.Sin(start_angle) * radius + center.Y,
                                  0)).ScaleFromOrigin(scale),
                    End = (new Point(
                                  Math.Cos(end_angle) * radius + center.X,
                                  Math.Sin(end_angle) * radius + center.Y,
                                  0)).ScaleFromOrigin(scale),
                    CW = false //DXF always defines them CCW
                }
            };
        }
        public static List<ArcSegment> BuildArcSegment(Point p1, double radius, double scale = 1)
        {                    
            //4x 90deg arcs is more compatible than longer arcs

            //for each 90deg of arc, use a new arc
            //TODO
            var arc_list = new List<ArcSegment>();

            arc_list.AddRange(BuildArcSegment(p1.ScaleFromOrigin(scale), radius * scale, BasicCAM.Geometry.Angle.Deg2Rad(0), BasicCAM.Geometry.Angle.Deg2Rad(90)));
            arc_list.AddRange(BuildArcSegment(p1.ScaleFromOrigin(scale), radius * scale, BasicCAM.Geometry.Angle.Deg2Rad(90), BasicCAM.Geometry.Angle.Deg2Rad(180)));
            arc_list.AddRange(BuildArcSegment(p1.ScaleFromOrigin(scale), radius * scale, BasicCAM.Geometry.Angle.Deg2Rad(180), BasicCAM.Geometry.Angle.Deg2Rad(270)));
            arc_list.AddRange(BuildArcSegment(p1.ScaleFromOrigin(scale), radius * scale, BasicCAM.Geometry.Angle.Deg2Rad(270), BasicCAM.Geometry.Angle.Deg2Rad(360)));

            return arc_list;
        }


    }
}

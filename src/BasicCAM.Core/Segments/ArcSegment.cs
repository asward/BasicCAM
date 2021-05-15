using System;
using System.Collections.Generic;
using System.Text;
using BasicCAM.Core.Geometry;
using BasicCAM.Core.GCode;
using BasicCAM.Core.Geometry;
using BasicCAM.Core.Exceptions;
using System.Linq;

namespace BasicCAM.Core.Segments
{
    using static Quadrants;
    using static Axes;

    [Serializable]
    public class ArcSegment : Segment
    {         
        protected ArcSegment()
        {

        }

        /// <summary>
        /// Arc from buldge
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="buldge"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public ArcSegment(Point p1, Point p2, double buldge, double scale = 1)
        {
            var theta = 4 * Math.Atan(buldge);
            var d = p1.To(p2).Magnitude / 2;
            var s = buldge * d;
            var r = (Math.Pow(s, 2) + Math.Pow(d, 2)) / (2 * s);


            var alpha = (Math.PI / 2 - Math.Abs(theta / 2)) * Math.Sign(buldge);
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

            Center = p1 + vectorToCenter;
            Radius = Math.Abs(r * scale);
            Start = p1.ScaleFromOrigin(scale);
            End = p2.ScaleFromOrigin(scale);
            CW = buldge < 0;
        }

        /// <summary>
        /// Arc from centre, radius, start point, end point, an cw
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="start_point"></param>
        /// <param name="end_point"></param>
        /// <param name="cw"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public ArcSegment(Point center, double radius, Point start_point, Point end_point, bool cw = true, double scale = 1)
        {
            Radius = radius * scale;
            Center = center.ScaleFromOrigin(scale);
            Start = start_point.ScaleFromOrigin(scale);
            End = end_point.ScaleFromOrigin(scale);
            CW = cw;
        }

        /// <summary>
        /// Arc from center, radius, start angle, end angle
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="start_angle"></param>
        /// <param name="end_angle"></param>
        /// <param name="cw"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public ArcSegment(Point center, double radius, double start_angle, double end_angle, bool cw = false, double scale = 1)
        {
            if (Math.Abs(end_angle % (2 * Math.PI)).Equals(start_angle % (2 * Math.PI)))
                throw new SegmentCreationException($"Arc start/end angles must be seperated by larger than {Double.Epsilon}");

            Radius = radius * scale;
            Center = center.ScaleFromOrigin(scale);
            Start = (new Point(
                            Math.Cos(start_angle) * radius + center.X,
                            Math.Sin(start_angle) * radius + center.Y,
                            0)).ScaleFromOrigin(scale);
            End = (new Point(
                            Math.Cos(end_angle) * radius + center.X,
                            Math.Sin(end_angle) * radius + center.Y,
                            0)).ScaleFromOrigin(scale);

            CW = cw;
        }

        /// <summary>
        /// Arc Segment from a dxf definition of arc
        /// </summary>
        /// <param name="dxfArc"></param>
        /// <param name="scale"></param>
        public ArcSegment(DXFReader.Core.Entities.Arc dxfArc, double scale = 1)
        {
            var startAngle = dxfArc.StartAngle * Math.PI / 180;
            var endAngle = dxfArc.EndAngle * Math.PI / 180;
            Layer = dxfArc.LayerName;
            Radius = dxfArc.Radius * scale;
            Center = (new Point(dxfArc.Center.X, dxfArc.Center.Y, dxfArc.Center.Z)).ScaleFromOrigin(scale);
            Start = (new Point(
                            Math.Cos(startAngle) * Radius + Center.X,
                            Math.Sin(startAngle) * Radius + Center.Y,
                            0)).ScaleFromOrigin(scale);
            End = (new Point(
                            Math.Cos(endAngle) * Radius + Center.X,
                            Math.Sin(endAngle) * Radius + Center.Y,
                            0)).ScaleFromOrigin(scale);
            CW = false;//DXF always defines them CCW
        }

        public override string Type { get; set; } = "Arc";
     
        public Point Center { get; protected set; }
        private double _radius;
        public double Radius
        {
            get { return _radius; } 
            protected set 
            { 
                //Radius reduced below center point, flip sense and keep radius positive
                if(value < 0)
                    CW = !CW;

                _radius = Math.Abs(value);
            } 
        }
        public bool CW { get; protected set; } = true;
        public override double StartAngle
        {
            get
            {
                var v = new Vector(Center, Start);
                var value = CW ? v.AngleX - Math.PI / 2 : v.AngleX + Math.PI / 2;
                return value % (2 * Math.PI);
            }
        }

        public override double EndAngle
        {
            get
            {
                var v = new Vector(Center, End);

                var value = CW ? v.AngleX - Math.PI / 2 : v.AngleX + Math.PI / 2;
                return value % (2 * Math.PI);
            }
        }

        public override double XMax
        {
            get
            {
                if (AxesCrossed().Contains(AXES.X_POS))
                    return Center.X + Radius;
                return Math.Max(End.X, Start.X);
            }
        }
        public override double YMax
        {
            get
            {
                if (AxesCrossed().Contains(AXES.X_POS))
                    return Center.Y + Radius;
                return Math.Max(End.Y, Start.Y);
            }
        }
        public override double XMin
        {
            get
            {
                if (AxesCrossed().Contains(AXES.X_NEG))
                    return Center.X - Radius;
                return Math.Min(End.X, Start.X);
            }
        }
        public override double YMin
        {
            get
            {
                if (AxesCrossed().Contains(AXES.Y_NEG))
                    return Center.Y - Radius;
                return Math.Min(End.Y, Start.Y);
            }
        }

        public override double Length
        {
            get
            {
                return Radius * AngleSwept;
            }
        }
        public double AngleSwept
        {
            get
            {
                double angleBetween;
                if (CW)
                {
                    angleBetween = -(EndAngle - StartAngle);
                }
                else
                {
                    angleBetween = (EndAngle - StartAngle);
                }
                if (angleBetween < 0)
                    angleBetween += 2 * Math.PI;

                return angleBetween;
            }
        }
        public IEnumerable<Point> PointsOnArc(int nPoints)
        {
            if (nPoints < 2)
                throw new ArgumentOutOfRangeException("nPoints must be 2 or greater");

            //Split arc into even arc lengths
            var arcStep = AngleSwept / (nPoints-1);

            if (CW)
                arcStep = -arcStep;

            var baseVector = new Vector(Center, Start);
            List<Point> approxPoints = new List<Point>();
            for (int nthPoint = 0; nthPoint < nPoints; nthPoint++)
            {
                yield return Center + baseVector.Rotate(arcStep * nthPoint);
            }
        }
        public override double ApproxWinding
        {
            get
            {
                List<Point> approxPoints = PointsOnArc(20).ToList();

                double sum = 0;
                for (int index = 0; index < approxPoints.Count - 1; index++)
                {
                    var p1 = approxPoints[index];
                    var p2 = approxPoints[index + 1];
                    sum += (p2.X - p1.X) * (p2.Y + p1.Y);
                }

                sum += (approxPoints[0].X - approxPoints[approxPoints.Count - 1].X) * (approxPoints[0].Y + approxPoints[approxPoints.Count - 1].Y);

                return sum;
            }
        }

        /// <summary>
        /// + Offset is larger radius, - is smaller 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public override Segment Offset(double offset)
        {
            //Offset with - value are left of direction
            //Offset with + value are right of direction

            //A        B       X              A B X
            //CW with  + is DECREASE radius   1 1 0
            //CW with  - is INCREASE radius   1 0 1
            //CCW with + is INCREASE radius   0 1 1
            //CCW with - is DECREASE radius   0 0 0

            //A XNOR B  is same as A == B
            var offsetIsPositive = offset >= 0;

            if(CW == offsetIsPositive)
            {
                //DECRESE RADIUS
                offset = -Math.Abs(offset);
            }
            else
            {
                //INCREASE RADIUS
                offset = Math.Abs(offset);
            }

            //Vectors through center to end points
            var startOffset = new Vector(Center, Start);
            var endOffset = new Vector(Center, End);

            //Normalize and scale to offset value
            startOffset.Normalize().Scale(offset);
            endOffset.Normalize().Scale(offset);

            //Apply offset to start/end points
            Start += startOffset;
            End += endOffset;

            //Change radius
            Radius += offset;

            return this;
        }
        public override Segment Reverse()
        {
            var temp = this.Start;
            this.Start = this.End;
            this.End = temp;

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
        /// Lists which local axes the Arc crosses 
        /// 1 - +x
        /// 2 - +y
        /// 3 - -x
        /// 4 - -y
        /// </summary>
        /// <returns></returns>
        public List<AXES> AxesCrossed()
        {
            var axesCrossed = new List<AXES>();

            QUADRANT startQuad = Quadrants.FindQuadrant(Start,Center);
            QUADRANT endQuad = Quadrants.FindQuadrant(End,Center);

            QUADRANT currentQuad = startQuad;

            while (endQuad != currentQuad)
            {
                if (CW)
                {
                    currentQuad = currentQuad.Previous();
                    axesCrossed.Add(currentQuad.NextAxis());
                }
                else
                {
                    currentQuad = currentQuad.Next();
                    axesCrossed.Add(currentQuad.PreviousAxis());
                }
            }

            return axesCrossed;
        }

        public override bool IsOnSegment(Point p)
        {
            //Arc 1
            //p1
            var a1Startp1 = new ArcSegment(Center, Radius, Start, p, CW);
            var a1p1End = new ArcSegment(Center, Radius, p, End, CW);
            return (a1Startp1.Length + a1p1End.Length).ToString("G10") == Length.ToString("G10");
        }

    }
}

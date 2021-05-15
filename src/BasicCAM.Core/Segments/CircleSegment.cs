using System;
using System.Collections.Generic;
using System.Text;
using BasicCAM.Core.Geometry;
using BasicCAM.Core.GCode;
using BasicCAM.Core.Geometry;

namespace BasicCAM.Core.Segments
{
    using static Quadrants;
    using static Axes;

    [Serializable]
    public class CircleSegment : ArcSegment
    {
        private CircleSegment()
        {

        }
        public CircleSegment(Point center, double radius, bool cw, double scale = 1)
        {
            Center = center.ScaleFromOrigin(scale);
            Radius = radius * scale;
            Start = new Point(Center.X + Radius, Center.Y, Center.Z);
            End = new Point(Center.X + Radius, Center.Y, Center.Z);
            CW = cw;
        }
        public CircleSegment(DXFReader.Core.Entities.Circle dxfCircle, double scale = 1)
        {
            Layer = dxfCircle.LayerName;

            Radius = dxfCircle.Radius * scale;
            Center = (new Point(dxfCircle.Center.X, dxfCircle.Center.Y, dxfCircle.Center.Z)).ScaleFromOrigin(scale);
            Start = new Point(Center.X + Radius, Center.Y, Center.Z);
            End = new Point(Center.X + Radius, Center.Y, Center.Z);
            CW = true; 
        }

        public override string Type { get; set; } = "Circle";
     
        public override double XMax
        {
            get
            {
                return Center.X + Radius;
            }
        }
        public override double YMax
        {
            get
            {
                return Center.Y + Radius;
            }
        }
        public override double XMin
        {
            get
            {
                return Center.X - Radius;
            }
        }
        public override double YMin
        {
            get
            {
                return Center.Y - Radius;
            }
        }

        private double? _length = null;
        public override double Length
        {
            get
            {
                return 2 * Math.PI * Radius;
            }
        }
        public override double ApproxWinding
        {
            get
            {
                //CW neg
                //CCW pos

                return CW ? 1 : -1;
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

            if (CW == offsetIsPositive)
            {
                //DECRESE RADIUS
                offset = -Math.Abs(offset);
            }
            else
            {
                //INCREASE RADIUS
                offset = Math.Abs(offset);
            }


            Radius += offset;

            var v = new Vector(Center, Start);
            v.Normalize().Scale(offset);

            Start = Start + v;
            End = End + v;
            return this;
        }
        public override Segment Reverse()
        {
            CW = !CW;

            return this;
        }
        public override bool IsOnSegment(Point p)
        {
            var v = new Vector(Center, p);
            return v.Magnitude == Radius;
        }
    }
}

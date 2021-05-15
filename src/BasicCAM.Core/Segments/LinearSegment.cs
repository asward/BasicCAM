using System;
using System.Collections.Generic;
using System.Text;
using BasicCAM.Core.Geometry;
using BasicCAM.Core.GCode;
using MathNet.Numerics.LinearAlgebra;

namespace BasicCAM.Core.Segments
{
    [Serializable]
    public class LinearSegment : Segment
    {

        private LinearSegment()
        {

        }
        public LinearSegment(Point start, Point end, double scale = 1)
        {
            Start = start.ScaleFromOrigin(scale);
            End = end.ScaleFromOrigin(scale);
        }
        public LinearSegment(DXFReader.Core.Entities.Line dxfLine, double scale = 1)
        {
            Layer = dxfLine.LayerName;
            Start = (new Point(dxfLine.Start.X, dxfLine.Start.Y, dxfLine.Start.Z)).ScaleFromOrigin(scale);
            End = (new Point(dxfLine.End.X, dxfLine.End.Y, dxfLine.End.Z)).ScaleFromOrigin(scale);
        }
        public override string Type { get; set; } = "Line";

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
                return Math.Sqrt(Math.Pow(End.X - Start.X, 2) + Math.Pow(End.Y - Start.Y, 2));
            }
        }

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
        public override double ApproxWinding
        {
            get
            {
                return (End.X - Start.X) * (End.Y + Start.Y);
            }
        }

        /// <summary>
        /// + Offset is left of segment, - is right
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public override Segment Offset(double offset)
        {
            //Offset with - value are left of direction
            //Offset with + value are right of direction
            var v = new Vector(Start, End);

            //Vector along path, rotated 90deg right and scaled offset magnitive
            v
               .Normalize()
               .Scale(offset)
               .Rotate(-Math.PI / 2);

            //Apply to start and end points
            Start = Start + v;
            End = End + v;
            return this;
        }
        public override Segment Shift(double x, double y, double z)
        {
            Start = new Point(Start.X + x, Start.Y + y, Start.Z + z);
            End = new Point(End.X + x, End.Y + y, End.Z + z);

            return this;
        }

        public override Segment Reverse()
        {
            var temp = this.Start;
            this.Start = this.End;
            this.End = temp;
            return this;
        }

        public override bool IsOnSegment(Point p)
        {
            var v1 = new Vector(Start, p);
            var v2 = new Vector(p, End);

            return (v1.Magnitude + v2.Magnitude).ToString("G10") == Length.ToString("G10");
        }
    }
}

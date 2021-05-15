using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicCAM.Core.Geometry;
using BasicCAM.Core.Preferences;
using BasicCAM.Core.GCode;
using MathNet.Numerics.LinearAlgebra;

namespace BasicCAM.Core.Segments
{
    /// <summary>
    /// Represensts geometery bounded by 2 end points
    /// </summary>
    [Serializable]
    public abstract class Segment
    {
        public string Id => Guid.NewGuid().ToString();
        public string Layer { get; set; } = "";
        public virtual string Type { get; set; } = "Segment";

        //public SegmentSettings Settings { get; set; }

        public Point Start { get; protected set; }
        public Point End { get; protected set; }

        public abstract double XMax { get; }
        public abstract double YMax { get; }
        public abstract double XMin { get; }
        public abstract double YMin { get; }
        public abstract double Length { get; }
        public abstract double StartAngle { get; }
        public abstract double EndAngle { get; }

        public abstract double ApproxWinding { get; }

        public virtual Segment ShallowCopy() 
        {
            return (Segment) this.MemberwiseClone();
        }
        public abstract Segment Shift(double x, double y, double z);

        /// <summary>
        /// Offsets segment by 'offset'. + values offset left, - values offset right
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public abstract Segment Offset(double offset);
        public abstract Segment Reverse();

        public virtual void SetStart(Point p)
        {
            Start = p;
        }

        public virtual void SetEnd(Point p)
        {
            End = p;
        }

        public abstract bool IsOnSegment(Point p);
    }

   
}

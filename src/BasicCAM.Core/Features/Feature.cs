using BasicCAM.Core.Geometry;
using BasicCAM.Core.Preferences;
using BasicCAM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicCAM.Core.Segments;

namespace BasicCAM.Core.Features
{
    //Block is a group of segments in order 
    [Serializable]
    public class Feature
    {
        private List<Segment> _segments = new List<Segment>();
        public IReadOnlyCollection<Segment> Segments => _segments;

        public FeatureSettings Settings { get; set; } = new FeatureSettings();
        public double XMax
        {
            get
            {
                return Segments.OrderBy(m => m.XMax).FirstOrDefault().XMax;
            }
        }
        public double YMax
        {
            get
            {
                return Segments.OrderBy(m => m.YMax).FirstOrDefault().YMax;
            }
        }
        public double XMin
        {
            get
            {
                return Segments.OrderByDescending(m => m.XMin).FirstOrDefault().XMin;
            }
        }
        public double YMin
        {
            get
            {
                return Segments.OrderByDescending(m => m.YMin).FirstOrDefault().YMin;
            }
        }

        /// <summary>
        /// Generate segments which trace the feature given the defined tool. Does not generate transition between paths (if any)
        /// </summary>
        /// <param name="toolDiameter"></param>
        /// <returns></returns>       
        public void AddSegment(Segment segment)
        {
            _segments.Add(segment);
        }

        public void ReverseDirection()
        {
            _segments.ReverseSegements();

        }
    }
}

using BasicCAM.Geometry;
using BasicCAM.Preferences;
using BasicCAM;
using BasicCAM.Segments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicCAM.Segments
{
    //Block is a group of segments in order 
    [Serializable]
    public class SegmentBlock
    {
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
        public string Name { get; set; } = "Block";
        public List<Segment> Segments = new List<Segment>();

        public BlockPreferences Preferences { get; set; } = new BlockPreferences();

        public bool IsClosedLoop
        {
            get
            {
                for (int i = 0; i < Segments.Count-1; i++)
                {
                    if (Segments[i].End.To(Segments[i + 1].Start).Magnitude > 0.0001) //TODO mathmatical lower limit, or preference setting
                        return false;
                }

                if (Segments[Segments.Count - 1].End.To(Segments[0].Start).Magnitude > 0.0001) //TODO mathmatical lower limit, or preference setting
                    return false;

                return true;

            }
        }

        public bool OrderIsCw
        {
            get
            {

                double sum = 0;
                foreach (var segment in Segments)
                {
                    sum += segment.ApproxWinding();
                }

                return sum > 0.0 ? true : false;
            }
        }
    }
}

using BasicCAM.Core.Geometry;
using BasicCAM.Core.Segments;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAM.Core.Solutions
{
    public class SolutionSegment<TSegment> where TSegment : Segment
    {

        public SolutionSegment(TSegment segment, SolutionSegmentSettings settings)
        {
            Segment = segment;
            Settings = new SolutionSegmentSettings(settings);
        }
        public static SolutionSegment<Segment> Linear(Point start, Point end, SolutionSegmentSettings settings)
        {
            var linearSegment = new LinearSegment(start, end);
            var solutionLinearSegment = new SolutionSegment<Segment>(linearSegment, settings);
            return solutionLinearSegment;
        }

        public TSegment Segment { get; }

        public SolutionSegmentSettings Settings { get; set; }
    }
}

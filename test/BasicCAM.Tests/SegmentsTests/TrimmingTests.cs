using BasicCAM.Core.Segments;
using BasicCAM.Core.Geometry;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BasicCAM.Tests.SegmentsTests
{
    public class Trimming
    {
        [Fact]
        public void LinearSegmentTrimsWithLinearSegment()
        {
            var l1 = new LinearSegment(new Point(1, 0, 0), new Point(-1, 0, 0));
            var l2 = new LinearSegment(new Point(0, 1, 0), new Point(0, -1, 0));

            List<Segment> segments = new List<Segment>() { l1, l2 };

            segments.TrimOverlapingSegments();
              
            l1.End.X.Should().BeApproximately(0,0.0001);
            l1.End.Y.Should().BeApproximately(0, 0.0001);
            l1.End.Z.Should().BeApproximately(0, 0.0001);

            l2.Start.X.Should().BeApproximately(0, 0.0001);
            l2.Start.Y.Should().BeApproximately(0, 0.0001);
            l2.Start.Z.Should().BeApproximately(0, 0.0001);
        }
    }
}

using BasicCAM.Core.Segments;
using BasicCAM.Core.Geometry;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BasicCAM.Tests.SegmentsTests
{
    public class Intersection
    {
        [Fact]
        public void LinearSegmentIntersectsWithLinearSegment()
        {
            var l1 = new LinearSegment(new Point(0, 0, 0), new Point(10, 10, 0));
            var l2 = new LinearSegment(new Point(0, 10, 0), new Point(10, 0, 0));

            var pResult = new Point();
            var result = l1.TryIntersection(l2, ref pResult);

            result.Should().Be(true);
            pResult.X.Should().Be(5);
            pResult.Y.Should().Be(5);
            pResult.Y.Should().Be(0);
        }
        [Fact]
        public void LinearSegmentsIntersectProperly()
        {
            var l1 = new LinearSegment(new Point(1, 0, 0), new Point(-1, 0, 0));
            var l2 = new LinearSegment(new Point(0, 1, 0), new Point(0, -1, 0));

            var result = BasicCAM.Core.Segments.Intersections.TryIntersection(l1, l2, out Point p);


            result.Should().BeTrue();
            p.Should().NotBeNull();

            p.X.Should().BeApproximately(0, 0.0001);
            p.Y.Should().BeApproximately(0, 0.0001);
            p.Z.Should().BeApproximately(0, 0.0001);
        }
    }
}

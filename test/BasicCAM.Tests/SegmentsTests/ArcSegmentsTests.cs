using BasicCAM.Core.Geometry;
using BasicCAM.Core.Segments;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace BasicCAM.Tests.NewFolder
{
    public class ArcSegmentsTests
    {
        [Fact]
        public void BuildArcSegmentsFromBuldge()
        {
            var p1 = new Point(2, 1.75, 0);
            var p2 = new Point(1.75, 2, 0);
            var buldge = 0.414213562373095;
           
            var arc = new ArcSegment(p1, p2, buldge);

            arc.Center.X.Should().BeApproximately(1.75,0.0001);
            arc.Center.Y.Should().BeApproximately(1.75, 0.0001);
            arc.Center.Z.Should().BeApproximately(0, 0.0001);

            arc.Radius.Should().BeApproximately(0.25, 0.0001);

            arc.CW.Should().BeFalse();

            arc.StartAngle.Should().BeApproximately(Math.PI / 2, 0.0001);
            arc.EndAngle.Should().BeApproximately(Math.PI, 0.0001);

            arc.Start.X.Should().Be(p1.X);
            arc.Start.Y.Should().Be(p1.Y);
            arc.Start.Z.Should().Be(p1.Z);

            arc.End.X.Should().Be(p2.X);
            arc.End.Y.Should().Be(p2.Y);
            arc.End.Z.Should().Be(p2.Z);
        }
        [Fact]
        public void CWArcSegmentPositiveOffsetDecreasesRadius()
        {
            var a = new ArcSegment(new Point(0, 0, 0), 1, 0, Math.PI,true);

            var radiusBefore = a.Radius;

            a.Offset(0.1);

            a.CW.Should().BeTrue();
            a.Radius.Should().BeLessThan(radiusBefore);
        }
        [Fact]
        public void CWArcSegmentNegativeOffsetIncreasesRadius()
        {
            var a = new ArcSegment(new Point(0, 0, 0), 1, 0, Math.PI, true);

            var radiusBefore = a.Radius;

            a.Offset(-0.1);

            a.CW.Should().BeTrue();
            a.Radius.Should().BeGreaterThan(radiusBefore);
        }

        [Fact]
        public void CCWArcSegmentPositiveOffsetIncreasesRadius()
        {
            var a = new ArcSegment(new Point(0, 0, 0), 1, Math.PI, 0,false);

            var radiusBefore = a.Radius;

            a.Offset(0.1);

            a.CW.Should().BeFalse();
            a.Radius.Should().BeGreaterThan(radiusBefore);
        }
        [Fact]
        public void CCWArcSegmentNegativeOffsetDecreasesRadius()
        {
            var a = new ArcSegment(new Point(0, 0, 0), 1, Math.PI, 0, false);

            var radiusBefore = a.Radius;

            a.Offset(-0.1);

            a.CW.Should().BeFalse();
            a.Radius.Should().BeLessThan(radiusBefore);
        }


        [Theory]
        [InlineData(false,0+Double.Epsilon, 2 * Math.PI- Double.Epsilon, 2 * Math.PI)] //CCW 0-2pi, should be 2pi
        [InlineData(false, 0 , Math.PI, Math.PI)] //CCW 0-pi, should be pi
        [InlineData(true, 2 * Math.PI - Double.Epsilon, 0 + Double.Epsilon, 2 * Math.PI)] //CW 2pi-0 should be 2pi
        [InlineData(true, 0, Math.PI, Math.PI)] //CW 0-pi, should be pi
        public void ArcLengthCalculatedAccuratley(bool cw, double startAngle, double endAngle, double expectedLength)
        {
            var center = new Point(0, 0, 0);
            var radius = 1;
            var arc = new ArcSegment(center, radius, startAngle, endAngle, cw);

            arc.Length.Should().BeApproximately(expectedLength, 0.0001);
        }

        [Fact]
        public void CWArcSegmentsPositiveOffsetDecreasesLength()
        {
            var a1 = new ArcSegment(new Point(0, 0, 0), 1, 0, Math.PI, true);

            var a2 = new ArcSegment(new Point(0, 0, 0), 1, Math.PI, 0, true);

            List<Segment> segments = new List<Segment>() { a1, a2 };

            var lengthBefore = segments.Length();

            segments.OffsetSegements(0.1);

            var lengthAfter = segments.Length();

            lengthAfter.Should().BeLessThan(lengthBefore);
        }
        [Fact]
        public void ArcReverses()
        {
            var a = new ArcSegment(new Point(0, 0, 0), 1, Math.PI, 0, false);

            var cwBefore = a.CW;
            var radiusBefore = a.Radius;
            var startBefore = a.Start;
            var endBefore = a.End;
            var lengthBefore = a.Length;

            a.Reverse();

            a.CW.Should().Be(!cwBefore);
            a.Radius.Should().Be(radiusBefore);
            a.Start.Should().Be(endBefore);
            a.End.Should().Be(startBefore);
            a.Length.Should().Be(lengthBefore);
        }
    }
}

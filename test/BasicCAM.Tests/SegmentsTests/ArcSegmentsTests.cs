using BasicCAM.Geometry;
using BasicCAM.Segments;
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
           
            var result = ArcSegment.BuildArcSegment(p1, p2, buldge);

            var arc = result.First();

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
    }
}

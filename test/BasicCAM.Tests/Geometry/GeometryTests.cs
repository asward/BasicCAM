﻿using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using BasicCAM.Core.Segments;
using BasicCAM.Core.Geometry;
using FluentAssertions;

namespace BasicCAM.Tests.Geometry
{
    public class GeometryTests
    {

        [Theory]
        [InlineData(false, 1, 0, 0, 1, Math.PI / 2, Math.PI)]
        [InlineData(false, 0, 1, -1, 0, Math.PI, Math.PI * 3 / 2)]
        [InlineData(false, -1, 0, 0, -1, Math.PI* 3/ 2, 0)]
        //[InlineData(true, 1, 0, 1, 1, Math.PI / 2, -Math.PI / 4)]
        //[InlineData(true, 1, 1, 1, 1, Math.PI * 3/4, -Math.PI / 4)]
        //[InlineData(true, 1, -1, -1, 1, Math.PI / 4, -Math.PI * 3 / 4)]
        public void ArcAnglesAreCorrect(bool cw, double x1, double y1, double x2, double y2, double expectedStart, double expectedEnd)
        {
            var Center = new Point(0, 0);
                var Radius = 1;
                var CW = cw;
                var Start = new Point(x1, y1);
                var End = new Point(x2, y2);
            var ArcSegment = new ArcSegment(Center, Radius, Start, End, CW);

            ArcSegment.StartAngle.Should().BeApproximately(expectedStart, 0.00001);
            ArcSegment.EndAngle.Should().BeApproximately(expectedEnd, 0.00001);
        }


        [Theory]
        [InlineData(0,0,0,1)]
        [InlineData(1, 0, 1, 1)]
        [InlineData(1,1, 1, 1)]
        [InlineData(1, -1, -1, 1)]
        public void LineAnglesAreCorrect(double x1, double y1, double x2, double y2)
        {

            var Start = new Point(x1, y1);
            var End = new Point(x2, y2);
            var lineSegment = new LinearSegment(Start, End);


            var expected = Math.Atan2((y2 - y1) , (x2 - x1));


            Assert.Equal(expected, lineSegment.StartAngle);
            Assert.Equal(lineSegment.StartAngle, lineSegment.EndAngle);
        }
    }
}

using BasicCAM.Core.Features;
using BasicCAM.Core.Geometry;
using BasicCAM.Core.Segments;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BasicCAM.Tests.Features
{
    
    public class FeatureTests
    {
        [Fact]
        public void CWFeatureOffsetOutsideCutIsNegativeOffset()
        {
            var feature = new Feature();
            var cwCircle = new CircleSegment(new Point(),1,true); //CW
            feature.Settings.CutSide = Core.Preferences.CutSide.Outside; //OUTSIDE

            feature.AddSegment(cwCircle);

            Math.Sign(feature.OffsetForTool(1)).Should().Be(-1); // NEGATIVE
        }

        [Fact]
        public void CCWFeatureOffsetOutsideCutIsPositiveOffset()
        {
            var feature = new Feature();
            var cwCircle = new CircleSegment(new Point(), 1, false); //CCW
            feature.Settings.CutSide = Core.Preferences.CutSide.Outside; //OUTSIDE

            feature.AddSegment(cwCircle);

            Math.Sign(feature.OffsetForTool(1)).Should().Be(1); //POSITIVE
        }

        [Fact]
        public void CWFeatureOffsetInsideCutIsPositiveOffset()
        {
            var feature = new Feature();
            var cwCircle = new CircleSegment(new Point(), 1, true); //CW
            feature.Settings.CutSide = Core.Preferences.CutSide.Inside; //INSIDE

            feature.AddSegment(cwCircle);

            Math.Sign(feature.OffsetForTool(1)).Should().Be(1); //POSITIVE
        }

        [Fact]
        public void CCWFeatureOffsetInsideCutIsNegativeOffset()
        {
            var feature = new Feature();
            var cwCircle = new CircleSegment(new Point(), 1, false); //CCW
            feature.Settings.CutSide = Core.Preferences.CutSide.Inside; //INSIDE

            feature.AddSegment(cwCircle);

            Math.Sign(feature.OffsetForTool(1)).Should().Be(-1); //NEGATIVE
        }
    }
}

using BasicCAM.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DXFReader.Core;
using BasicCAM.Core.Segments;
using BasicCAM.Core.Features;

namespace BasicCAM.Core.Features
{
    public interface IFeatureBuilder
    {

        IEnumerable<Feature> Features(IEnumerable<Segment> segments);
    }
}

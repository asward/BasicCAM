
using BasicCAM.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicCAM;
using System.IO;
using DXFReader.Core;
using BasicCAM.Core.Segments;
using BasicCAM.Core.Features;
using BasicCAM.Core.Preferences;

namespace BasicCAM.Core.Features
{
    public abstract class FeatureBuilder : IFeatureBuilder
    {
        protected double scale => Document_Preferences.ScaleFactor;
        protected double featureTolerance => Document_Preferences.FeatureTolerance;
        protected string outsideLayer => Document_Preferences.OutsideLayer;
        public IDocument_Preferences Document_Preferences { get; }

        public FeatureBuilder(IDocument_Preferences document_Preferences) 
        {
            Document_Preferences = document_Preferences;
        }
        public IEnumerable<Feature> Features(IEnumerable<Segment> segments)
        {
            Point startingPoint = new Point();

            var feature = new Feature();
            
            List<Segment> remainingSegments = new List<Segment>(segments);

            while (remainingSegments.Any())
            {
                var next = remainingSegments
                    .OrderBy(m => Math.Min(m.Start.To(startingPoint).Magnitude, m.End.To(startingPoint).Magnitude))
                    .First();

                var distance_start = next.Start.To(startingPoint).Magnitude;
                var distance_end = next.End.To(startingPoint).Magnitude;

                if (next.Length < featureTolerance)
                {
                    remainingSegments.Remove(next);
                    continue;
                }

                if (distance_start > distance_end)
                    next.Reverse();

                startingPoint = next.End;

                //Nearest outside of chianing tolerance, start a new chain.
                if (distance_start > featureTolerance && distance_end > featureTolerance && feature.Segments.Any())
                {
                    finishFeature(feature);

                    yield return feature;

                    feature = new Feature();
                }

                feature.AddSegment(next);
                remainingSegments.Remove(next);
            }

            if (feature.Segments.Any())
            {
                finishFeature(feature);

                yield return feature;
            }

            void finishFeature(Feature feature)
            {
                feature.Settings.CutSide = CutSide.Inside;
                if (!feature.Segments.IsCW())
                {
                    feature.ReverseDirection();
                }

                //if (!feature.IsClosedLoop)
                //{
                //    feature.Settings.CutSide = CutSide.Center;
                //    return;
                //}

                //if (feature.Segments.All(s => s.Layer.ToLower() == outsideLayer.ToLower()))
                //{
                //    feature.Settings.CutSide = CutSide.Outside;
                //}
                //else
                //{
                //    feature.Settings.CutSide = CutSide.Inside;
                //}
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BasicCAM.Core.Features;
using BasicCAM.Core.Geometry;
using BasicCAM.Core.Preferences;
using BasicCAM.Core.Segments;
using DXFReader.Core;
//using DXFReader.Core.Entities;


namespace BasicCAM.Core.Features
{
    public class DXFFeatureBuilder : FeatureBuilder, IFeatureBuilder
    {
        //Class to wrap DXF Document and create 'standardized' shapes list.
        public  DXFFeatureBuilder(IDocument_Preferences document_Preferences) 
            : base(document_Preferences)
        {
        }
        public  async Task<IEnumerable<Feature>> FromBytes(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return await FromStream(stream);
            }
        }
        public  async Task<IEnumerable<Feature>> FromStream(Stream stream)
        {
            DXFDocument dxfDocument = new DXFDocument();

            await dxfDocument.LoadFileAsync(stream);

            return Features(dxfDocument);
        }

        public IEnumerable<Feature> Features(DXFDocument document)
        {
            var segments = Segments(document);
            foreach(var feature in Features(segments))
            {
                yield return feature;
            }
        }
        public IEnumerable<Segment> Segments(DXFReader.Core.Entities.Entity entity)
        {
            
            switch (entity)
            {
                case DXFReader.Core.Entities.Line line:
                    yield return new LinearSegment(line);
                    break;
                case DXFReader.Core.Entities.Arc arc:
                    yield return new ArcSegment(arc);
                    break;
                case DXFReader.Core.Entities.Circle circle:
                    yield return new CircleSegment(circle);
                    break;
                case DXFReader.Core.Entities.LWPolyLine pl:
                    Point v0 = new Point(pl.Vertices[0].X, pl.Vertices[0].Y, pl.Vertices[0].Z);
                    Point v1 = new Point(pl.Vertices[1].X, pl.Vertices[1].Y, pl.Vertices[1].Z);
                    double b0 = pl.Buldge[0];
                    for (int i = 1; i <= pl.NumPoints - 1; i++)
                    {
                        v1 = new Point(pl.Vertices[i].X, pl.Vertices[i].Y, pl.Vertices[i].Z); 

                        if (b0 != 0)
                        {
                            yield return new ArcSegment(v0, v1, b0, scale);
                        }
                        else
                        {
                            yield return new LinearSegment(v0, v1, scale);
                        }

                        v0 = v1;
                        b0 = pl.Buldge[i];
                    }

                    if (pl.PolylineFlag == 1)
                    {
                        v1 = new Point(pl.Vertices[0].X, pl.Vertices[0].Y, pl.Vertices[0].Z);
                        if (b0 != 0)
                        {
                            yield return new ArcSegment(v0, v1, b0, scale);
                        }
                        else
                        {
                            yield return new LinearSegment(v0, v1, scale);
                        }
                    }
                    break;
                default:
                    throw new NotSupportedException($"Unsupported DXF type {entity.GetType()}");
                    break;
            }


        }
        public IEnumerable<Segment> Segments(DXFDocument document)
        {
            foreach (var entity in document.Entities.EntityList_2D)
            {
                foreach (var segment in Segments(entity))
                {
                    yield return segment;
                }
            }
        }
    }
}

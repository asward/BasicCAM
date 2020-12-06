using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BasicCAM.Geometry;
using BasicCAM.Segments;
using DXFReader.Core;
using DXFReader.Core.Entities;
using Line = DXFReader.Core.Entities.Line;
using Point = BasicCAM.Geometry.Point;

namespace BasicCAM.Interpreter
{
    public class DXF_Interpreter : DocumentInterpreterBase, IDocumentInterpreter
    {
        public double DocumentScale { get; set; } = 1;
        //Class to wrap DXF Document and create 'standardized' shapes list.
        public  DXF_Interpreter()
        {

        }

        public async Task LoadStream(Stream stream, double scale = 1)
        {
            DXFDocument dxfDocument = new DXFDocument();

            await dxfDocument.LoadFileAsync(stream);

            LoadDocument(dxfDocument, scale);
        }
        public override void LoadDocument(DXFDocument document, double scale = 1)
        {
            DocumentScale = scale;

            foreach (var e in document.Entities.EntityList_2D)
            {
                if (e is Arc)
                {
                    var a = e as Arc;
                    var radius = a.Radius;
                    var center = new Point(a.Center.X, a.Center.Y, a.Center.Z);
                    var start_angle = Angle.Deg2Rad(a.StartAngle);
                    var end_angle = Angle.Deg2Rad(a.EndAngle);
                    var am = ArcSegment.BuildArcSegment(center, radius, start_angle, end_angle, DocumentScale);

                    Segments.AddRange(am);

                }
                else if (e is Circle)
                {
                    var c = e as Circle;

                    var Radius = c.Radius;
                    var Center = new Point(c.Center.X, c.Center.Y, c.Center.Z);

                    var am = ArcSegment.BuildArcSegment(Center, Radius, DocumentScale);

                    Segments.AddRange(am);
                }
                else if (e is Line)
                {
                    var dxf_line = e as Line;

                    var p1 = new Point(dxf_line.Start.X, dxf_line.Start.Y, dxf_line.Start.Z);
                    var p2 = new Point(dxf_line.End.X, dxf_line.End.Y, dxf_line.End.Z);

                    var lm = LinearSegment.BuildLinearSegment(p1, p2, DocumentScale);

                    Segments.AddRange(lm);
                }
                else if (e is LWPolyLine)
                {
                    var pl = e as LWPolyLine;
                    Point v0 = new Point(pl.Vertices[0].X, pl.Vertices[0].Y, pl.Vertices[0].Z);
                    Point v1 = new Point(pl.Vertices[1].X, pl.Vertices[1].Y, pl.Vertices[1].Z);
                    double b0 = pl.Buldge[0];
                    for (int i = 1; i <= pl.NumPoints - 1; i++)
                    {
                        v1 = new Point(pl.Vertices[i].X, pl.Vertices[i].Y, pl.Vertices[i].Z); //TODO repeated

                        if (b0 != 0)
                        {
                            var am = ArcSegment.BuildArcSegment(v0, v1, b0, DocumentScale);

                            Segments.AddRange(am);
                        }
                        else
                        {
                            var lm = LinearSegment.BuildLinearSegment(v0, v1, DocumentScale);
                            Segments.AddRange(lm);
                        }

                        v0 = v1;
                        b0 = pl.Buldge[i];
                    }

                    if (pl.PolylineFlag == 1)
                    {
                        v1 = new Point(pl.Vertices[0].X, pl.Vertices[0].Y, pl.Vertices[0].Z);
                        if (b0 != 0)
                        {
                            var am = ArcSegment.BuildArcSegment(v0, v1, b0, DocumentScale);

                            Segments.AddRange(am);
                        }
                        else
                        {
                            var lm = LinearSegment.BuildLinearSegment(v0, v1, DocumentScale);
                            Segments.AddRange(lm);
                        }
                    }
                }
                else if (e is PolyLine)
                {
                    throw new Exception("PolyLine not supported!");
                }

                //TODO SUPPORT SPLINE, ELLIPSE, POLYLINE ETC....
            }
        }
    }
}

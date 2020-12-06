using BasicCAM.Segments;
using BasicCAM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicCAM;
using System.IO;
using DXFReader.Core; 

namespace BasicCAM.Interpreter
{
    public abstract class DocumentInterpreterBase
    {
        public List<Segment> Segments { get; set; } = new List<Segment>();
        public string FilePath { get; set; }
        //public abstract void LoadDocument(string filePath, double scale = 1);
        public abstract void LoadDocument(DXFDocument document, double scale = 1);

        public double Width
        {
            get
            {

                return Segments.Max(d => d.XMax) - Segments.Min(d => d.XMin);

            }
        }
        public double Heigth
        {
            get
            {

                return Segments.Max(d => d.YMax) - Segments.Min(d => d.YMin);

            }
        }
        public Point Center
        {
            get
            {
                var x = (Segments.Max(d => d.XMax) + Segments.Min(d => d.XMin))/2;
                var y = (Segments.Max(d => d.YMax) + Segments.Min(d => d.YMin))/ 2;

                return new Point(x, y, 0);
            }
        }

     
    }
}

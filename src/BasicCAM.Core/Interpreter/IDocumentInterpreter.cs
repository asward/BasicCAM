using BasicCAM.Segments;
using BasicCAM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DXFReader.Core; 

namespace BasicCAM.Interpreter
{
    public interface IDocumentInterpreter
    {
         List<Segment> Segments { get; set; }
         string FilePath { get; set; }
         void LoadDocument(DXFDocument document, double scale);
         double Width { get; }
         double Heigth { get; }
         Point Center { get; }
    }
}

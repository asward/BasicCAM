using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicCAM.Preferences
{
    public interface IDocument_Preferences
    {
        public string InsideLayer { get; set; } 
        public string OutsideLayer { get; set; }
        public double ScaleFactor { get; set; } 
        public double X_Centering_Offset { get; set; }
        public double Y_Centering_Offset { get; set; }
    }
}

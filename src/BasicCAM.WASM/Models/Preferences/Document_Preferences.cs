using BasicCAM.Preferences;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BasicCAMWebAppWASM.Models.Preferences
{
    [Serializable]
    public class Document_Preferences : IDocument_Preferences
    {
        [DisplayName("Inside Layer")]
        public string InsideLayer { get; set; } = "INSIDE";
        public string OutsideLayer { get; set; } = "OUTSIDE";
        public double ScaleFactor { get; set; } = 1.000;
        public double X_Centering_Offset { get; set; } = 0.000;
        public double Y_Centering_Offset { get; set; } = 0.000;
    }
}

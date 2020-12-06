using BasicCAM.Preferences;
using BasicCAM.GCode;
using System;
using System.ComponentModel;

namespace BasicCAMWebAppWASM.Models.Preferences
{
    [Serializable]
    public class GCode_Preferences : IGCode_Preferences
    {
        [Category("Commands")]
        [DisplayName("Rapid Movement")]
        [Description("G-Code for rapid movement")]
        public GCodeWord LinearRapid { get; set; } = "G0";
        public GCodeWord Linear { get; set; } = "G1";
        public GCodeWord ArcCW { get; set; } = "G2";
        public GCodeWord ArcCCW { get; set; } = "G3";
        public GCodeWord Rehome { get; set; } = "G28";
        public string ToolOn { get; set; } = "M106";
        public string ToolOff { get; set; } = "M107";

        public int SignificantFigures { get; set; } = 5;
        public bool IncludeLineNumbers { get; set; } = true;
        public string StartCommentDelimeter { get; set; } = "(";
        public string EndCommentDelimeter { get; set; } = ")";
        public string F { get; set; } = "F";
        public string S { get; set; } = "S";
        public string X { get; set; } = "X";
        public string Y { get; set; } = "Y";
        public string Z { get; set; } = "Z";
        public string U { get; set; } = "U";
        public string V { get; set; } = "V";
        public string W { get; set; } = "W";
        public string I { get; set; } = "I";
        public string J { get; set; } = "J";
        public string K { get; set; } = "K";
        public string R { get; set; } = "R";
        public string G { get; set; } = "G";
        public string M { get; set; } = "M";
    }
}

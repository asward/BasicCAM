using BasicCAM.Preferences;
using BasicCAM.GCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAM.Preferences
{
    class Marlin_GCode_Preferences : IGCode_Preferences
    {
        public GCodeWord LinearRapid { get; set; } = "G00";
        public GCodeWord Linear { get; set; } = "G01";
        public GCodeWord ArcCW { get; set; } = "G02";
        public GCodeWord ArcCCW { get; set; } = "G03";
        public GCodeWord Rehome { get; set; } = "G28";
        public string ToolOn { get; set; } = "M4";
        public string ToolOff { get; set; } = "M3";
        public int SignificantFigures { get;  set; } = 5;
        public string StartCommentDelimeter { get; set; } = "(";
        public string EndCommentDelimeter { get; set; } = ")";
        public bool IncludeLineNumbers { get; set; } = false;

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

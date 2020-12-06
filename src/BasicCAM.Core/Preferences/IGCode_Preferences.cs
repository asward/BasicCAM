using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using BasicCAM.GCode;

namespace BasicCAM.Preferences
{
   
   
    public interface IGCode_Preferences
    {
        //RAPID MOVMENT
        GCodeWord LinearRapid { get; set; } //G0

        //LINEAR MOVEMENT
        GCodeWord Linear { get; set; } //G1

        //ARC MOVEMENT - CW
        GCodeWord ArcCW { get; set; } //G2

        //ARC MOVEMENT - CCW
        GCodeWord ArcCCW { get; set; } //G3

        //AUTOHOME
        GCodeWord Rehome { get; set; } //G28



        string ToolOn { get; set; } //M3 or 106
        string ToolOff { get; set; } //M4 or 107
        string F { get; set; }
        string S { get; set; }
        string X { get; set; }
        string Y { get; set; }
        string Z { get; set; }
        string U { get; set; }
        string V { get; set; }
        string W { get; set; }
        string I { get; set; }
        string J { get; set; }
        string K { get; set; }
        string R { get; set; }
        string G { get; set; }
        string M { get; set; }

        int SignificantFigures { get; set; } 
        string StartCommentDelimeter { get; set; }

        string EndCommentDelimeter { get; set; }

        bool IncludeLineNumbers { get; set; }
    }
}

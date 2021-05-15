using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using BasicCAM.Core.Enums;
using BasicCAM.Core.GCode;

namespace BasicCAM.Core.Preferences
{
    public class GCode_Preferences : IGCode_Preferences
    {
        [DisplayName("Linear Rapid")]
        [Description("Code for linear rapid movment")]
        [RegularExpression(@"^G[\d]*$", ErrorMessage = "Code starts with G followed by digits. No other characters or spaces")]
        public virtual string LinearRapid { get; set; } = "G0";

        [DisplayName("Linear")]
        [Description("Code for linear movment")]
        [RegularExpression(@"^G[\d]*$", ErrorMessage = "Code starts with G followed by digits. No other characters or spaces")]
        public virtual string Linear { get; set; } = "G1";

        [DisplayName("Arc CW")]
        [Description("Code for clockwise arc movment")]
        [RegularExpression(@"^G[\d]*$", ErrorMessage = "Code starts with G followed by digits. No other characters or spaces")]
        public virtual string ArcCW { get; set; } = "G2";

        [DisplayName("Arc CCW")]
        [Description("Code for counter-clockwise arc movment")]
        [RegularExpression(@"^G[\d]*$", ErrorMessage = "Code starts with G followed by digits. No other characters or spaces")]
        public virtual string ArcCCW { get; set; } = "G3";


        [DisplayName("Auto Home")]
        [Description("Code for auto home routine")]
        [RegularExpression(@"^G[\d]*$", ErrorMessage = "Code starts with G followed by digits. No other characters or spaces")]
        public virtual string AutoHome { get; set; } = "G28";

        [DisplayName("Comment Start Delimeter")]
        [Description("String for starting a comment")]
        public virtual string StartCommentDelimeter { get; set; } = "(";

        [DisplayName("Comment End Delimeter")]
        [Description("String for ending a comment")]
        public virtual string EndCommentDelimeter { get; set; } = ")";

        [DisplayName("Include Line Numbers")]
        [Description("Include line numbers in GCode output")]
        public virtual bool IncludeLineNumbers { get; set; } = true;

        [DisplayName("Line Number Increment")]
        [Description("Increment between GCode output lines")]
        [Range(1, int.MaxValue, ErrorMessage = "Value must be between {1} and {2}")]
        public virtual int LineNumberIncrement { get; set; } = 1;


        [DisplayName("Comment Level")]
        [Description("Level of commenting for GCode Output")]
        public virtual COMMENT_LEVEL CommentLevel { get; set; } = COMMENT_LEVEL.NONE;

        [DisplayName("Header")]
        [Description("String to inlucde as header on GCode output")]
        public virtual string Header { get; set; } = "";

        [DisplayName("Footer")]
        [Description("String to inlucde as footer on GCode output")]
        public virtual string Footer { get; set; } = "";

        [DisplayName("Precision")]
        [Description("Precision (number of digits after decimal) to include in GCode output")]
        [Range(1, 20, ErrorMessage = "Value must be between {1} and {2}")]
        public virtual int Precision { get; set; } = 5;

        [DisplayName("Tool On")]
        [Description("Code for turing tool on")]
        [RegularExpression(@"^M[\d]*$", ErrorMessage = "Code starts with M followed by digits. No other characters or spaces")]
        public virtual string ToolOn { get; set; } = "M106";//M3 or 106


        [DisplayName("Tool Off")]
        [Description("Code for turing tool on")]
        [RegularExpression(@"^M[\d]*$", ErrorMessage = "Code starts with M followed by digits. No other characters or spaces")]
        public virtual string ToolOff { get; set; } = "M107"; //M4 or 107


        [DisplayName("Feedrate")]
        [Description("Variable used for feedrate")]
        [RegularExpression(@"^[A-Z]$", ErrorMessage = "Code must be single character A-Z")]
        public virtual string F { get; set; } = "F";

        [DisplayName("Speed")]
        [Description("Variable used for (tool) speed")]
        [RegularExpression(@"^[A-Z]$", ErrorMessage = "Code must be single character A-Z")]
        public virtual string S { get; set; } = "S";

        [DisplayName("X")]
        [Description("Variable used for X")]
        [RegularExpression(@"^[A-Z]$", ErrorMessage = "Code must be single character A-Z")]
        public virtual string X { get; set; } = "X";

        [DisplayName("Y")]
        [Description("Variable used for Y")]
        [RegularExpression(@"^[A-Z]$", ErrorMessage = "Code must be single character A-Z")]
        public virtual string Y { get; set; } = "Y";

        [DisplayName("Z")]
        [Description("Variable used for Z")]
        [RegularExpression(@"^[A-Z]$", ErrorMessage = "Code must be single character A-Z")]
        public virtual string Z { get; set; } = "Z";

        [DisplayName("U")]
        [Description("Variable used for incremental X")]
        [RegularExpression(@"^[A-Z]$", ErrorMessage = "Code must be single character A-Z")]
        public virtual string U { get; set; } = "U";

        [DisplayName("V")]
        [Description("Variable used for incremental Y")]
        [RegularExpression(@"^[A-Z]$", ErrorMessage = "Code must be single character A-Z")]
        public virtual string V { get; set; } = "V";

        [DisplayName("W")]
        [Description("Variable used for incremental Z")]
        [RegularExpression(@"^[A-Z]$", ErrorMessage = "Code must be single character A-Z")]
        public virtual string W { get; set; } = "W";

        [DisplayName("I")]
        [Description("Variable used for relative arc center in X")]
        [RegularExpression(@"^[A-Z]$", ErrorMessage = "Code must be single character A-Z")]
        public virtual string I { get; set; } = "I";

        [DisplayName("J")]
        [Description("Variable used for relative arc center in Y")]
        [RegularExpression(@"^[A-Z]$", ErrorMessage = "Code must be single character A-Z")]
        public virtual string J { get; set; } = "J";

        [DisplayName("K")]
        [Description("Variable used for relative arc center in Z")]
        [RegularExpression(@"^[A-Z]$", ErrorMessage = "Code must be single character A-Z")]
        public virtual string K { get; set; } = "K";

        [DisplayName("R")]
        [Description("Variable used for arc raidus")]
        [RegularExpression(@"^[A-Z]$", ErrorMessage = "Code must be single character A-Z")]
        public virtual string R { get; set; } = "R";

    }

    public interface IGCode_Preferences
    {
        string ArcCCW { get; set; }
        string ArcCW { get; set; }
        string AutoHome { get; set; }
        COMMENT_LEVEL CommentLevel { get; set; }
        string EndCommentDelimeter { get; set; }
        string F { get; set; }
        string Footer { get; set; }
        string Header { get; set; }
        string I { get; set; }
        bool IncludeLineNumbers { get; set; }
        string J { get; set; }
        string K { get; set; }
        string Linear { get; set; }
        string LinearRapid { get; set; }
        int LineNumberIncrement { get; set; }
        int Precision { get; set; }
        string R { get; set; }
        string S { get; set; }
        string StartCommentDelimeter { get; set; }
        string ToolOff { get; set; }
        string ToolOn { get; set; }
        string U { get; set; }
        string V { get; set; }
        string W { get; set; }
        string X { get; set; }
        string Y { get; set; }
        string Z { get; set; }
    }
}

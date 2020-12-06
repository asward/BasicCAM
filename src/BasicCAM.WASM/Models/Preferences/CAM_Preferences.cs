using BasicCAM.Preferences;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BasicCAMWebAppWASM.Models.Preferences
{
    [Serializable]
    public class CAM_Preferences : ICAM_Preferences
    {

        [Category("Commenting")]
        [DisplayName("CommentLevel")]
        [Description("Maximum output comment Level.")]
        public COMMENT_LEVEL CommentLevel { get; set; } = COMMENT_LEVEL.ERROR;

        public string Header { get; set; } = "";
        public string Footer { get; set; } = "";
        public double ToolDiameter { get; set; } = 0.0;

        [Range(1, 100, ErrorMessage = "Passes must be between 1 and 100.")]
        public int Passes { get; set; } = 1;
        public double CutFeedrate { get; set; } = 100;
        public double RapidFeedrate { get; set; } = 500;
        public string ToolOnSequence { get; set; } = "M106";
        public string ToolOffSequence { get; set; } = "M107";
        public bool ToolOffDuringRapid { get; set; } = true;
        public int ToolSpeedPowerMin { get; set; } = 0;
        public int ToolSpeedPowerMax { get; set; } = 255;
        public int ToolSpeedPower { get; set; } = 255;
        public double RapidZLevelDelta { get; set; } = 0;

        //public string G0 { get; set; } = "G0";
        //public string G1 { get; set; } = "G1";
        //public string G2 { get; set; } = "G2";
        //public string G3 { get; set; } = "G3";
        //public string G28 { get; set; } = "G28";
        public double Cut_Z_Level { get; set; } = 0.0;
        public double Rapid_Z_Level { get; set; } = 0.0;
        public double ScaleFactor { get; set; } = 1.0;
        public bool EnableComments { get; set; } = true;

        public double Start_X { get; set; } = 0.0;
        public double Start_Y { get; set; } = 0.0;
        public double Start_Z { get; set; } = 0.0;

        public bool InitializeAtStartPoisiton { get; set; } = true;
        public bool StartPointIsHome { get; set; } = true;
        public bool HomeX { get; set; } = true;
        public bool HomeY { get; set; } = true;
        public bool HomeZ { get; set; } = true;
        public double MachineTolerance { get; set; } = 0.0000001;
        
        public bool IncludeComments { get; set; } = true;

        public bool AutoCenter { get; set; } = true;
        public int RoundingDigits { get; set; } = 5;
        public bool ToolOffDuringPlunge { get; set; } = false;
        public double ToolSpeedPowerPercentage { get; set; } = 100;
    }
}

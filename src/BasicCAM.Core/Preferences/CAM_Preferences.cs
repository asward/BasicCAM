using BasicCAM.Core.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BasicCAM.Core.Preferences
{
    public class CAM_Preferences : ICAM_Preferences
    {

        [DisplayName("Cutting Feedrate")]
        [Description("Feedrate to use for cutting")]
        [Range(0, double.MaxValue, ErrorMessage = "Must be {1} or greater")]
        public virtual double CutFeedrate { get; set; } = 100;


        [DisplayName("Rapid Feedrate")]
        [Description("Feedrate to use for rapid moves")]
        [Range(0, double.MaxValue, ErrorMessage = "Must be {1} or greater")]
        public virtual double RapidFeedrate { get; set; } = 500;


        [DisplayName("Passes")]
        [Description("Number of passes for entire project")]
        [Range(1, 100, ErrorMessage = "Passes must be between {1} and {2}.")]
        public virtual int Passes { get; set; } = 1;

        [DisplayName("Tool Off During Rapid")]
        [Description("Turns the tool off during rapids")]
        public virtual bool ToolOffDuringRapid { get; set; } = true;

        [DisplayName("Tool Speed/Power Minimum")]
        [Description("Minimum speed/power for the tool (for grayscale engrave only)")]
        [DisableDisplay]
        public virtual int ToolSpeedPowerMin { get; set; } = 0;

        [DisplayName("Tool Speed/Power Maximum")]
        [Description("Maximum speed/power for the tool (for grayscale engrave only)")]
        [DisableDisplay]
        public virtual int ToolSpeedPowerMax { get; set; } = 255;


        [DisplayName("Tool Speed/Power")]
        [Description("Speed/power for the tool during cuts")]
        public virtual int ToolSpeedPower { get; set; } = 255;


        [DisplayName("Rapid Z Level")]
        [Description("Z value to perform rapids")]
        public virtual double RapidZLevel { get; set; } = 0;

        [DisplayName("Cut Z Level")]
        [Description("Z value to perform cuts. Used as first cut with Z stepovers")]
        public virtual double CutZLevel { get; set; } = 0.0;

        [DisplayName("Scale Factor")]
        [Description("Scaling of output")]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be greater than {1}")]
        [DisableDisplay]
        public virtual double ScaleFactor { get; set; } = 1.0;


        [DisplayName("Start X")]
        [Description("Starting X position")]
        public virtual double StartX { get; set; } = 0.0;

        [DisplayName("Start Y")]
        [Description("Starting Y position")]
        public virtual double StartY { get; set; } = 0.0;

        [DisplayName("Start Z")]
        [Description("Starting Z position")]
        public virtual double StartZ { get; set; } = 0.0;

        [DisplayName("Home X")]
        [Description("Home X position")]
        public virtual double HomeX { get; set; } = 0.0;

        [DisplayName("Home Y")]
        [Description("Home Y position")]
        public virtual double HomeY { get; set; } = 0.0;

        [DisplayName("Home Z")]
        [Description("Home Z position")]
        public virtual double HomeZ { get; set; } = 0.0;

        [DisplayName("Start At Home")]
        [Description("Start routine from the Home position")]
        public virtual bool StartAtHome { get; set; } = false;


        [DisplayName("Finish At Home")]
        [Description("Finish routine at the Home position")]
        public virtual bool FinishAtHome { get; set; } = false;


        [DisplayName("Initialize At Start")]
        [Description("Moves to the start postiion before begining the program")]
        public virtual bool InitializeAtStartPoisiton { get; set; } = true;


        [DisplayName("Precision")]
        [Description("CAM solution precision.")]
        [Range(0, int.MaxValue, ErrorMessage = "Value must be greater than {1}")]
        public virtual int Precision { get; set; } = 5;

        [DisplayName("Tool Off During Plunge")]
        [Description("Turn the tool off during pluge movements")]
        public virtual bool ToolOffDuringPlunge { get; set; } = false;



        [DisplayName("Auto Center")]
        [Description("Shift features to be centered in work area")]
        [Range(0.0000000001, double.MaxValue, ErrorMessage = "Value must be greater than {1}")]
        public virtual bool AutoCenter { get; set; } = true;


        [DisplayName("Centering Offset X")]
        [Description("Length in X to shift ")]
        public virtual double X_Centering_Offset { get; set; }
        public virtual double Y_Centering_Offset { get; set; }



    }


    public interface ICAM_Preferences
    {
        bool AutoCenter { get; set; }
        double CutFeedrate { get; set; }
        double CutZLevel { get; set; }
        bool FinishAtHome { get; set; }
        double HomeX { get; set; }
        double HomeY { get; set; }
        double HomeZ { get; set; }
        bool InitializeAtStartPoisiton { get; set; }
        int Passes { get; set; }
        int Precision { get; set; }
        double RapidFeedrate { get; set; }
        double RapidZLevel { get; set; }
        double ScaleFactor { get; set; }
        bool StartAtHome { get; set; }
        double StartX { get; set; }
        double StartY { get; set; }
        double StartZ { get; set; }
        bool ToolOffDuringPlunge { get; set; }
        bool ToolOffDuringRapid { get; set; }
        int ToolSpeedPower { get; set; }
        int ToolSpeedPowerMax { get; set; }
        int ToolSpeedPowerMin { get; set; }
        double X_Centering_Offset { get; set; }
        double Y_Centering_Offset { get; set; }
    }
}

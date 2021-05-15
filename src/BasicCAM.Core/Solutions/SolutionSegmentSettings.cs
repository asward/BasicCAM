using BasicCAM.Core.Features;
using BasicCAM.Core.Geometry;
using BasicCAM.Core.Preferences;
using BasicCAM.Core.Segments;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAM.Core.Solutions
{
    [Serializable]
    public class SolutionSegmentSettings
    {
        public SolutionSegmentSettings(BasicCAMProject project, Feature feature)
        {
            Feedrate = Math.Abs(project.CAM_Preferences.CutFeedrate * feature.Settings.FeedrateFactor) ;

            ToolSpeedPower = Math.Abs((int) (project.CAM_Preferences.ToolSpeedPowerMax * feature.Settings.ToolSpeedPowerFactor));
            Plane = PLANE.XY;
            Rapid = false;
            ToolOn = true;
        }
        public SolutionSegmentSettings(SolutionSegmentSettings settings) 
        {
            Feedrate = settings.Feedrate;
            ToolSpeedPower = settings.ToolSpeedPower;
            Plane = settings.Plane;
            Rapid = settings.Rapid;
            ToolOn = settings.ToolOn;

            Comments = settings.Comments;
        }

        public SolutionSegmentSettings(double feedrate, bool rapid, bool toolOn, int toolSpeedPower)
        {
            Feedrate = feedrate;
            Rapid = rapid;
            ToolOn = toolOn;
            ToolSpeedPower = toolSpeedPower;
        }

        public double Feedrate { get; set; } = 0;
        public int ToolSpeedPower { get; set; } = 0;
        public PLANE Plane { get; set; } = PLANE.XY;
        public bool Rapid { get; set; } = false;
        public bool ToolOn { get; set; } = false;
        public string Comments { get; set; } = "";
    }
}

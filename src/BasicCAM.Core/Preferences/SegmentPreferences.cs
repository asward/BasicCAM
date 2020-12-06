using BasicCAM.Geometry;
using BasicCAM.Segments;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAM.Preferences
{
    [Serializable]
    public struct SegmentCAMSettings
    {
        public SegmentCAMSettings(SegmentCAMSettings settings)
        {
            Plane = settings.Plane;
            Rapid = settings.Rapid;

            ToolOn = settings.ToolOn;
            Feedrate = settings.Feedrate;
            ToolSpeedPower = settings.ToolSpeedPower;
            Comments = settings.Comments;
        }
        public PLANE Plane { get; set; }
        public bool Rapid { get; set; }
        public bool ToolOn { get; set; }
        public double Feedrate { get; set; }
        public int ToolSpeedPower { get; set; }
        public string Comments { get; set; }
    }
}

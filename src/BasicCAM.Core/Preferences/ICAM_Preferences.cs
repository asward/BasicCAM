using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicCAM.Preferences
{
    public interface ICAM_Preferences
    {
        public COMMENT_LEVEL CommentLevel { get; set; }

        public string Header { get; set; }

        public string Footer { get; set; }

        public double ToolDiameter { get; set; }

        public int Passes { get; set; }

        public double Start_X { get; set; }

        public double Start_Y { get; set; }

        public double Start_Z { get; set; }

        public bool InitializeAtStartPoisiton { get; set; }

        public bool StartPointIsHome { get; set; }

        public bool HomeX { get; set; }

        public bool HomeY { get; set; }

        public bool HomeZ { get; set; }

        public double RapidFeedrate { get; set; }

        public double CutFeedrate { get; set; }
        public string ToolOnSequence { get; set; }

        public string ToolOffSequence { get; set; }

        public bool ToolOffDuringRapid { get; set; }

        public bool ToolOffDuringPlunge { get; set; }
        public int ToolSpeedPowerMin { get; set; }

        public int ToolSpeedPowerMax { get; set; }

        public double ToolSpeedPowerPercentage { get; set; }

        public double RapidZLevelDelta { get; set; }




        public double Cut_Z_Level { get; set; }

        public double Rapid_Z_Level { get; set; }

        public double ScaleFactor { get; set; }

        public bool EnableComments { get; set; }




        public double MachineTolerance { get; set; }

        public int RoundingDigits { get; set; }

        public bool IncludeComments { get; set; }

        public bool AutoCenter { get; set; }
    }
    public enum COMMENT_LEVEL
    {
        DEBUG = 40,
        INFO = 10,
        WARNING = 20,
        ERROR = 30,
        NONE = 100
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAM.Core.GCode
{
    public class GCodeState
    {
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public double Z { get; set; } = 0;
        public double F { get; set; } = 0;
        public bool ToolOn { get; set; } = false;
        public string Type { get; set; } = "";
        public string ModalType { get; set; } = "";
        public int ToolPower { get; internal set; }
    }
}

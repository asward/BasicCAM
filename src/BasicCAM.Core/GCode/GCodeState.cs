using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAM.GCode
{
    public struct GCodeState
    {
        public double X;
        public double Y;
        public double Z;
        public double F;
        public bool ToolOn;
        public string Type;
        public string ModalType;
    }
}

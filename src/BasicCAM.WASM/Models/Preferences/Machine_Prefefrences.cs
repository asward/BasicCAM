using BasicCAM.Preferences;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAMWebAppWASM.Models.Preferences
{
    [Serializable]
    public class Machine_Preferences : IMachine_Preferences
    {
        public double Tool_X_Offset { get; set; } = 0.0;
        public double Tool_Y_Offset { get; set; } = 0.0;
        public double WorkArea_X { get; set; } = 0;
        public double WorkArea_Y { get; set; } = 0;
        public double WorkArea_Z { get; set; } = 800.0; 
        public double MachineableWidth
        {
            get
            {
                return WorkArea_X - Math.Abs(Tool_X_Offset);
            }
        }
        public double MachineableHeight
        {
            get
            {
                return WorkArea_Y - Math.Abs(Tool_Y_Offset);
            }
        }

        public double X_Centering_Offset { get; set; } = 0.0;
        public double Y_Centering_Offset { get; set; } = 0.0;
    }
}

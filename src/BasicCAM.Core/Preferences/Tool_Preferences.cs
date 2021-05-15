using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BasicCAM.Core.Preferences
{
    public class Tool_Preferences : ITool_Preferences
    {
        [DisplayName("Tool Diameter")]
        [Description("Tool's diameter")]
        public virtual double ToolDiameter { get; set; } = 0.016;
    }
    public interface ITool_Preferences
    {
        public double ToolDiameter { get; set; }
    }
}

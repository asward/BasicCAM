using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BasicCAM.Core.Preferences
{
    public class Machine_Preferences : IMachine_Preferences
    {
        [DisplayName("Machine Tolerance")]
        [Description("Tolerances of the machine.")]
        [Range(0.0000000001, double.MaxValue, ErrorMessage = "Value must be greater than {1}")]
        public virtual double MachineTolerance { get; set; } = 0.0000001;


        [DisplayName("Tool X Offset")]
        [Description("X axis offset of tool")]
        public virtual double Tool_X_Offset { get; set; } = 0;

        [DisplayName("Tool Y Offset")]
        [Description("Y axis offset of tool")]
        public virtual double Tool_Y_Offset { get; set; } = 0;


        [DisplayName("Work Area X")]
        [Description("Length in X axis of working area of the machine")]
        public virtual double WorkArea_X { get; set; } = 14;

        [DisplayName("Work Area Y")]
        [Description("Length in Y axis of working area of the machine")]
        public virtual double WorkArea_Y { get; set; } = 14;

        [DisplayName("Work Area Z")]
        [Description("Length in Z axis of working area of the machine")]
        public virtual double WorkArea_Z { get; set; } = 14;
    }


    public interface IMachine_Preferences
    {
        double MachineTolerance { get; set; }
        double Tool_X_Offset { get; set; }
        double Tool_Y_Offset { get; set; }
        double WorkArea_X { get; set; }
        double WorkArea_Y { get; set; }
        double WorkArea_Z { get; set; }
    }
}

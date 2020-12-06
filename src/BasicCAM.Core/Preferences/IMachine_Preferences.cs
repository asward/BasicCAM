namespace BasicCAM.Preferences
{
    public interface IMachine_Preferences
    {
        public double Tool_X_Offset { get; set; } 
        public double Tool_Y_Offset { get; set; } 
        public double WorkArea_X { get; set; } 
        public double WorkArea_Y { get; set; } 
        public double WorkArea_Z { get; set; }

        public double X_Centering_Offset { get; set; }
        public double Y_Centering_Offset { get; set; }
        public double MachineableWidth { get; }
        public double MachineableHeight { get; }
    }
}

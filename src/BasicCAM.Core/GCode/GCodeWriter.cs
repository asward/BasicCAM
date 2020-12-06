using BasicCAM.Segments;
using BasicCAM.Geometry;
using BasicCAM.Preferences;
using BasicCAM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicCAM.GCode
{
    public class GCodeWriter
    {
        private GCodeState State;
        private readonly BasicCAMSolution Solution;

        private StreamWriter sw;

        public GCodeWriter(BasicCAMSolution solution)
        {
            this.Solution = solution;

            
        }
        private void InitState()
        {
            State = new GCodeState()
            {
                ToolOn = false,
            };
        }
        private async Task ToggleTool(Segment segment)
        {
            if (!State.ToolOn && segment.Settings.ToolOn) //OFF, but SET ON
            {
                await ToolOn(segment.Settings.ToolSpeedPower);
            }
            else if (State.ToolOn && !segment.Settings.ToolOn) //ON, but SET OFF
            {
                await ToolOff();
            }
        }

        private async Task ToolOn(int power)
        {
            //TURN ON TOOL/LASER
            await WriteLineAsync(
                GCodeLine.FromWord(Solution.Project.GCode_Preferences.ToolOn)
                    .Word((Solution.Project.GCode_Preferences.S, power)));

            State.ToolOn = true;
        }

        private async Task ToolOff()
        {
            //TURN OFF TOOL/LASER
            await WriteLineAsync(
                GCodeLine.FromWord(Solution.Project.GCode_Preferences.ToolOff));

            State.ToolOn = false;
        }

        public async Task<string> WriteToFileAsync(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            using (sw = new StreamWriter(fs))
            {                
                //PROJECT PREAMBLE - TODO

                //Solution Segments
                foreach (var segment in Solution.Segments)
                {
                    await WriteSegmentAsync(segment);
                }

                await ToolOff();

                //PROJECT POSTAMBLE - TODO
            }

            return filePath;
        }
        public async Task WriteToStreamAsync(Stream stream)
        {
            sw = new StreamWriter(stream);

            //Solution Segments
            foreach (var segment in Solution.Segments)
            {
                await WriteSegmentAsync(segment);
            }

            await ToolOff();

            //PROJECT POSTAMBLE - TODO
        }
        private async Task WriteSegmentAsync(Segment segment)
        {
            await ToggleTool(segment);

            switch (segment)
            {
                case LinearSegment l:
                    await WriteLinearSegmentAsync(l);
                    return;
                case ArcSegment a:
                    await WriteArcSegmentAsync(a);
                    return;
                default:
                    return;
            }
        }
        private async Task WriteLinearSegmentAsync(LinearSegment segment)
        {
            var line = new GCodeLine(segment.Settings.Rapid ? Solution.Project.GCode_Preferences.LinearRapid: Solution.Project.GCode_Preferences.Linear);

            line.Words.Add((Solution.Project.GCode_Preferences.X, segment.End.X));
            line.Words.Add((Solution.Project.GCode_Preferences.Y, segment.End.Y));
            line.Words.Add((Solution.Project.GCode_Preferences.Z, segment.End.Z));
            line.Words.Add((Solution.Project.GCode_Preferences.F, segment.Settings.Feedrate));

            await WriteLineAsync(line);
        }
        private async Task WriteArcSegmentAsync(ArcSegment segment)
        {
            var line = new GCodeLine(segment.CW ? Solution.Project.GCode_Preferences.ArcCW : Solution.Project.GCode_Preferences.ArcCCW);

            Vector ijk = segment.Start.To(segment.Center);

            line.Words.Add((Solution.Project.GCode_Preferences.X, segment.End.X));
            line.Words.Add((Solution.Project.GCode_Preferences.Y, segment.End.Y));
            line.Words.Add((Solution.Project.GCode_Preferences.Z, segment.End.Z));
            line.Words.Add((Solution.Project.GCode_Preferences.I, ijk.X));
            line.Words.Add((Solution.Project.GCode_Preferences.J, ijk.Y));
            line.Words.Add((Solution.Project.GCode_Preferences.K, ijk.Z));

            line.Words.Add((Solution.Project.GCode_Preferences.F, segment.Settings.Feedrate));

            await WriteLineAsync(line);
        }

        private async Task WriteLineAsync(GCodeLine line)
        {
            string output = "";
            foreach (var word in line.Words)
            {
                output += WordToString(word);
            }

            if (!String.IsNullOrEmpty(line.Comments))
                output += $"{Solution.Project.GCode_Preferences.StartCommentDelimeter}{line.Comments}{Solution.Project.GCode_Preferences.EndCommentDelimeter}";

            await sw.WriteLineAsync(output);
        }

        string WordToString(GCodeWord word)
        {
            double tol_val = RoundToTolernace(word.Value, Solution.Project.CAM_Preferences.RoundingDigits);
            string val_string = WriteToString(tol_val, Solution.Project.GCode_Preferences.SignificantFigures);

            return $"{word.Type}{val_string}";
        }

        private string WriteToString(double d, int sig_digits)
        {
            return ((Decimal)Double.Parse(d.ToString($"G"))).ToString();
        }

        private double RoundToTolernace(double d, int tol_digits)
        {
            return Math.Round(d, tol_digits);
        }
    }
}

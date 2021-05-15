using BasicCAM.Core.Segments;
using BasicCAM.Core.Geometry;
using BasicCAM.Core.Preferences;
using BasicCAM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicCAM.Core.Solutions;

namespace BasicCAM.Core.GCode
{
    public class GCodeOutput
    {
        public GCodeState State { get; private set; } = new GCodeState();
        public List<GCodeLine> GCode { get; private set; } = new List<GCodeLine>();

        private readonly IGCode_Preferences preferences;

        public GCodeOutput(IGCode_Preferences preferences)
        {
            this.preferences = preferences;
        }

        /// <summary>
        /// Reset GCode and clear state
        /// </summary>
        public GCodeOutput Reset()
        {
            GCode.Clear();
            State = new GCodeState();
            return this;
        }

        /// <summary>
        /// Add segments to running gcode process
        /// </summary>
        /// <param name="segments"></param>
        public GCodeOutput AddSegments(IEnumerable<SolutionSegment<Segment>> segments)
        {
            foreach(var segment in segments)
            {
                SetTool(segment.Settings.ToolOn,segment.Settings.ToolSpeedPower);

                GCode.AddRange(ProcessSegmentAsync(segment));
            }

            return this;
        }

        /// <summary>
        /// Sets to to provided state
        /// </summary>
        /// <param name="setOn"></param>
        /// <param name="speedPower"></param>
        public void SetTool(bool setOn, int speedPower)
        {
            if (!State.ToolOn && setOn) //OFF and SET ON
            {
                 ToolOn(speedPower);
            }
            else if (State.ToolOn && !setOn) //ON, but SET OFF
            {
                 ToolOff();
            }
            else if (State.ToolPower != speedPower)  //ON, but power differs
            {
                ToolOn(speedPower);
            }
        }

        /// <summary>
        /// Adds a GCode line for turning the tool ON to provided POWER setting
        /// </summary>
        /// <param name="power"></param>
        public void ToolOn(int power)
        {
            State.ToolOn = true;

            State.ToolPower = power;

            GCode.Add(GCodeLine.FromWord(preferences.ToolOn)
                .Word(preferences.S, power));
        }

        /// <summary>
        /// Adds a GCode line for turning the tool OFF
        /// </summary>
        public void ToolOff()
        {
            State.ToolOn = false;

            GCode.Add(GCodeLine.FromWord(preferences.ToolOff));

        }

        /// <summary>
        /// Adds a line of GCode for the given segment motion
        /// </summary>
        /// <param name="segment"></param>
        public IEnumerable<GCodeLine> ProcessSegmentAsync(SolutionSegment<Segment> segment)
        {
            switch (segment.Segment)
            {
                case CircleSegment c:
                    return ProcessArcSegment(segment);
                case ArcSegment a:
                    return ProcessArcSegment(segment);
                case LinearSegment l:
                     return ProcessLinearSegment(segment);
                default:
                    throw new NotImplementedException($"Unkown segment type {typeof(Segment)}") ;
            }
        }

        /// <summary>
        /// Adds a line of GCode for a linear segment
        /// </summary>
        /// <param name="segment"></param>
        public IEnumerable<GCodeLine> ProcessLinearSegment(SolutionSegment<Segment> solutionSegment)
        {
            var segment = solutionSegment.Segment as LinearSegment;
            var line = new GCodeLine(solutionSegment.Settings.Rapid ? preferences.LinearRapid: preferences.Linear);

            line.Id = segment.Id;

            line.Words.Add((preferences.X, segment.End.X));
            line.Words.Add((preferences.Y, segment.End.Y));
            line.Words.Add((preferences.Z, segment.End.Z));
            line.Words.Add((preferences.F, solutionSegment.Settings.Feedrate));

            yield return line;
        }

        /// <summary>
        /// Adds a line of GCode for an arc segment
        /// </summary>
        /// <param name="segment"></param>
        public IEnumerable<GCodeLine> ProcessArcSegment(SolutionSegment<Segment> solutionSegment)
        {
            var segment = solutionSegment.Segment as ArcSegment;
            var line = new GCodeLine(segment.CW ? preferences.ArcCW : preferences.ArcCCW);

            line.Id = segment.Id;

            Vector ijk = segment.Start.To(segment.Center);

            line.Words.Add((preferences.X, segment.End.X));
            line.Words.Add((preferences.Y, segment.End.Y));
            line.Words.Add((preferences.Z, segment.End.Z));
            line.Words.Add((preferences.I, ijk.X));
            line.Words.Add((preferences.J, ijk.Y));
            line.Words.Add((preferences.K, ijk.Z));

            line.Words.Add((preferences.F, solutionSegment.Settings.Feedrate));

            yield return line;
        }

        /// <summary>
        /// Adds a line of GCode for a circle segment
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public IEnumerable<GCodeLine> ProcessCircleSegment(SolutionSegment<Segment> solutionSegment)
        {
            var segment = solutionSegment.Segment as CircleSegment;
            var line = new GCodeLine(segment.CW ? preferences.ArcCW : preferences.ArcCCW);

            line.Id = segment.Id;

            Vector ijk = segment.Start.To(segment.Center);

            line.Words.Add((preferences.I, ijk.X));
            line.Words.Add((preferences.J, ijk.Y));
            line.Words.Add((preferences.K, ijk.Z));

            line.Words.Add((preferences.F, solutionSegment.Settings.Feedrate));

            yield return line;
        }

        /// <summary>
        /// Writes GCode to specfied filePath
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<string> WriteToFileAsync(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            using (var sw = new StreamWriter(fs))
            {
                foreach(var (guid, output) in GUIDOutput())
                {
                    await sw.WriteLineAsync(output);
                }
            }

            return filePath;
        }

        /// <summary>
        /// Provides enumerable list of GUID,GCode value tuples
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string,string>> GUIDOutput()
        {
            int lineIndex = 0;

            string format(string line)
            {
                if (!preferences.IncludeLineNumbers)
                    return line;

                return $"N{(lineIndex++)*preferences.LineNumberIncrement} {line}";
            }

            //HEADER
            if (!String.IsNullOrWhiteSpace(preferences.Header))
            {
                foreach (var headerLine in preferences.Header.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
                {
                    yield return new KeyValuePair<string, string>("", format(headerLine));
                }
            }

            //BODY
            foreach (var line in GCode)
            {
                var bodyLine= line.ToString(preferences.Precision, preferences.StartCommentDelimeter, preferences.EndCommentDelimeter);
                yield return new KeyValuePair<string, string>(line.Id ?? "", format(bodyLine));
            }

            //FOOTER
            if (!String.IsNullOrWhiteSpace(preferences.Footer))
            {
                foreach (var footerLine in preferences.Footer.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
                {
                    yield return new KeyValuePair<string, string>("", format(footerLine));
                }
            }
        }
    }
}

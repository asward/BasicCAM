using BasicCAM.Core.GCode;
using BasicCAM.Core.Geometry;
using BasicCAM.Core.Preferences;
using BasicCAM.Core.Segments;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicCAM.Core.Features;
using System.Diagnostics;

namespace BasicCAM.Core.Solutions
{
    /// <summary>
    /// Generates final segment list which links together all blocks of project
    /// Combines and finalises segment settings (Tool on/off, feedrate, etc.)
    /// </summary>
    public class CAMSolution
    {
        public readonly BasicCAMProject Project;

        public List<SolutionSegment<Segment>> SolutionSegments { get; private set; } = new List<SolutionSegment<Segment>>();

        public CAMSolution(BasicCAMProject project)
        {
            this.Project = project;
        }
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public Task SolveAsync()
        {
            SolutionSegments.Clear();

            foreach (var feature in Project.Features)
            {
                //GET CUTPATH (INCLUDING MULTI PASS)
                var solutionSegments = BuildToolPath(feature);

                //INSERT TRANSITION BETWEEN GAPS
                InsertPlunges(solutionSegments);

                //LEADIN/OUT
                if (feature.Settings.InsertLeadIn)
                    InsertLeadIn(solutionSegments);

                if (feature.Settings.InsertLeadOut)
                    InsertLeadOut(solutionSegments);

                //LINK TO PREVIOUS FEATURE
                if (SolutionSegments.Any()) //Previous features exist
                {
                    InsertTransitions(solutionSegments);
                }
                else
                {
                    InsertStartSequence(solutionSegments);
                }
                

                //Add To Solution
                SolutionSegments.AddRange(solutionSegments);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Builds tool path for given feature. Clones, offsets, multipass (with stepover)
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public List<SolutionSegment<Segment>> BuildToolPath(Feature feature)
        {
            var toolRadius = Math.Abs(Project.Tool_Preferences.ToolDiameter) / 2;
            var passes = feature.Settings.Passes;
            var passoffset = feature.Settings.ZStepOver;

            var cutPathSegments = feature.Segments
                .Clone() //Clone to not modify existing segments
                .ShiftSegments(new Vector(0, 0, Project.CAM_Preferences.CutZLevel)) //Shift to cut level //TODO shift to center/offset
                .OffsetSegements(toolRadius,feature.Settings.CutSide)//Offset everything
                .TrimOverlapingSegments() //Trim if overlaps
                .FillGaps(); //Fill gaps created
            
            //Multipass
            if (passes > 1)
                cutPathSegments.LinearPatternFeature(passes, new Vector(0,0,passoffset));

            //Create as solution segments
            var feautresSegmentSettings = new SolutionSegmentSettings(Project, feature);
            var solutionSegments = cutPathSegments
                .Select(s => new SolutionSegment<Segment>(s, new SolutionSegmentSettings(feautresSegmentSettings)))
                .ToList();

            return solutionSegments;
        }

        /// <summary>
        /// Locate gaps in segments list and insert PLUNG segments to fill the gaps
        /// </summary>
        /// <param name="segments"></param>
        public void InsertPlunges(List<SolutionSegment<Segment>> segments)
        {
            //Trim overlapped segments
            for (int i = segments.Count - 1; i > 0; i--)
            {
                var aheadIndex = i;
                var behindIndex = i - 1;

                var ahead = segments[aheadIndex];
                var behind = segments[behindIndex];

                //If points line up, no need for transition
                if (ahead.Segment.Start.PracticallyEquals(behind.Segment.End)) 
                    continue;

                //If within tolerance of machine, no need for transition
                if (ahead.Segment.Start.Near(behind.Segment.End, Project.Machine_Preferences.MachineTolerance)) 
                    continue;

                //Insert a plunge after the behindIndex from behind.End to ahead.Start
                //Linear, tool off, as rapid
                var transitionSegment = new LinearSegment(start: behind.Segment.End, end: ahead.Segment.Start);
                var transitionSolutionSegment = new SolutionSegment<Segment>(transitionSegment, CreatePlunge(behind.Settings));

                segments.Insert(aheadIndex, transitionSolutionSegment);
            }
        }

        /// <summary>
        /// Create SegmentSettings as a rapid, based on previous settings
        /// </summary>
        /// <param name="previousSegmentSettings"></param>
        /// <returns></returns>
        public SolutionSegmentSettings CreateRapid(SolutionSegmentSettings previousSegmentSettings) 
        {
            return new SolutionSegmentSettings(previousSegmentSettings)
            {
                Rapid = true,
                ToolOn = !Project.CAM_Preferences.ToolOffDuringRapid,
                Comments = "Rapid",
            };
        }

        /// <summary>
        /// Create SegmentSettings as a plunge, based on previous settings
        /// </summary>
        /// <param name="previousSegmentSettings"></param>
        /// <returns></returns>
        public SolutionSegmentSettings CreatePlunge(SolutionSegmentSettings previousSegmentSettings)
        {
            return new SolutionSegmentSettings(previousSegmentSettings)
            {
                Rapid = false,
                ToolOn = !Project.CAM_Preferences.ToolOffDuringPlunge,
                Comments = "Plunge",
            };
        }

        /// <summary>
        /// Generate segments for rapid to start of segment list, from origin
        /// </summary>
        /// <param name="segments"></param>
        public void InsertStartSequence(List<SolutionSegment<Segment>> segments)
        {
            if (!segments.Any())
                return;

            SolutionSegmentSettings startSettings = new SolutionSegmentSettings(
                feedrate: Project.CAM_Preferences.RapidFeedrate,
                rapid: true,
                toolOn: false, 
                toolSpeedPower: Project.CAM_Preferences.ToolSpeedPowerMax //TODOD?
            );

            var firstSegment = segments.First();
            var startSequence = new List<SolutionSegment<Segment>>(); 

            Point startPoint = new Point(Project.CAM_Preferences.StartX, Project.CAM_Preferences.StartY, Project.CAM_Preferences.StartZ);
            Point endPoint = startPoint; //moving from whereever we were initially

            //Move to start position
            if (Project.CAM_Preferences.InitializeAtStartPoisiton)
            {
                var moveToStart = SolutionSegment<Segment>.Linear(startPoint, endPoint, CreateRapid(startSettings));
                startSequence.Add(moveToStart);
            }

            //Move to first segments start X/Y but don't plunge
            startPoint = endPoint;
            endPoint = new Point(firstSegment.Segment.Start.X, firstSegment.Segment.Start.Y, Project.CAM_Preferences.StartZ); 
            var rapidToFirstSegment = SolutionSegment<Segment>.Linear(startPoint, endPoint, CreateRapid(startSettings)); //From where everywhere are (start-start)
            startSequence.Add(rapidToFirstSegment);

            //Plunge to working Z
            startPoint = endPoint;
            endPoint = firstSegment.Segment.Start;
            if (!endPoint.PracticallyEquals(startPoint))
            {
                var plungeToDepth = SolutionSegment<Segment>.Linear(startPoint, endPoint, CreateRapid(startSettings)); //From where everywhere are (start-start)
                startSequence.Add(plungeToDepth);
            }
            //Insert into segment list
            segments.InsertRange(0, startSequence);
        }

        /// <summary>
        /// Adds transition segments between end of existing Motions to start of segment list
        /// </summary>
        /// <param name="segments"></param>
        public void InsertTransitions(List<SolutionSegment<Segment>> segments)
        {
            if (!segments.Any())
                return;

            Point previousSegmentEndPoint = SolutionSegments.Last().Segment.End;//Last SOLTUION segment
            SolutionSegmentSettings lastSettings = SolutionSegments.Last().Settings;
            var transitionSegments = new List<SolutionSegment<Segment>>();
            //Lift from last segment ending
            Point startPoint = previousSegmentEndPoint;
            Point endPoint = new Point(previousSegmentEndPoint.X, previousSegmentEndPoint.Y, Project.CAM_Preferences.RapidZLevel);
            if(!endPoint.PracticallyEquals(startPoint))
            {
                var liftFromLast = SolutionSegment<Segment>.Linear(startPoint, endPoint, CreateRapid(lastSettings));
                transitionSegments.Add(liftFromLast);
            }
            
            //Move to current segments start
            startPoint = endPoint;
            endPoint = new Point(segments.First().Segment.Start.X, segments.First().Segment.Start.Y, Project.CAM_Preferences.RapidZLevel);
            var rapidFromLast = SolutionSegment<Segment>.Linear(startPoint, endPoint, CreateRapid(lastSettings));
            transitionSegments.Add(rapidFromLast);
        

            //Plunge to starting location
            startPoint = endPoint;
            endPoint = segments.First().Segment.Start;
            if (!endPoint.PracticallyEquals(startPoint))
            {
                var plungeToDepth = SolutionSegment<Segment>.Linear(startPoint, endPoint, CreateRapid(lastSettings));
                transitionSegments.Add(plungeToDepth);
            }

            //Insert into segment list
            segments.InsertRange(0, transitionSegments);
        }

        /// <summary>
        /// Insert Lead in segments to segment list
        /// </summary>
        /// <param name="segments"></param>
        public void InsertLeadIn(List<SolutionSegment<Segment>> segments)
        {
            //TODO InsertLeadIn
            return;
        }

        /// <summary>
        /// Insert Lead out segments to segment list
        /// </summary>
        /// <param name="segments"></param>
        public void InsertLeadOut(List<SolutionSegment<Segment>> segments)
        {
            //TODO InsertLeadOut  
            return;
        }
    }
}

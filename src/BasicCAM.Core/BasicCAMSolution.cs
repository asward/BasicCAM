using BasicCAM.Geometry;
using BasicCAM.Preferences;
using BasicCAM.Segments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicCAM
{
    /// <summary>
    /// Generates final segment list which links together all blocks of project
    /// Combines and finalises segment settings (Tool on/off, feedrate, etc.)
    /// </summary>
    public class BasicCAMSolution
    {
        public readonly BasicCAMProject Project;

        public List<Segment> Segments = new List<Segment>();
        public BasicCAMSolution(BasicCAMProject project)
        {
            this.Project = project;
        }

        public void Solve()
        {
            Segments.Clear();

            foreach (var block in Project.BlockList)
            {
                //Clone segments to new list - combine settings and logic from project / block
                var segments = CloneAndInitializeSettings(block);

                //Offset for tool width, etc.
                if (block.IsClosedLoop && block.Preferences.CutSide != CutSide.Center) //Offset only applies to closed loops and non-center cuts
                    OffsetSegements(segments, block.Preferences.CutSide, block.OrderIsCw, Project.CAM_Preferences.ToolDiameter);

                //Add passes and transtions
                MultiPassSegments(segments, block.Preferences.Passes, block.Preferences.Passoffset, block.IsClosedLoop);

                //Add Lead-in and out
                if (block.Preferences.InsertLeadIn)
                    LeadInOut(segments);

                //Link segment list to previous list with rapid
                LinkWithRapids(segments);

                //Add To Solution
                Segments.AddRange(segments);
            }


        }


        private List<Segment> CloneAndInitializeSettings(SegmentBlock block)
        {
            var segments = block.Segments.Clone();

            foreach(var segment in segments)
            {
                segment.Settings = new SegmentCAMSettings() 
                {
                    Plane = PLANE.XY,
                    Rapid = false,
                    ToolOn = true,
                    Feedrate = Project.CAM_Preferences.CutFeedrate*block.Preferences.FeedRateFactor, 
                    ToolSpeedPower = Convert.ToInt32(Project.CAM_Preferences.ToolSpeedPowerMax* Project.CAM_Preferences.ToolSpeedPowerPercentage/100* block.Preferences.PowerFactor), //TODO calc
                    Comments = "" 
                };
            }
            return segments;
        }

        private SegmentCAMSettings GetRapidSettings(SegmentCAMSettings previousSegmentSettings) {
            return new SegmentCAMSettings(previousSegmentSettings)
            {
                Rapid = true,
                ToolOn = !Project.CAM_Preferences.ToolOffDuringRapid,
                Comments = "Rapid",
            };
        }

        private SegmentCAMSettings GetPlungeSettings(SegmentCAMSettings previousSegmentSettings)
        {
            return new SegmentCAMSettings(previousSegmentSettings)
            {
                Rapid = false,
                ToolOn = !Project.CAM_Preferences.ToolOffDuringPlunge,
                Comments = "Plunge",
            };
        }
        private void RapidToStart(List<Segment> segments)
        {
            SegmentCAMSettings startSettings = new SegmentCAMSettings()
            {
                Feedrate = Project.CAM_Preferences.RapidFeedrate,
                Plane = PLANE.XY,
                Rapid = true,
                ToolOn = false, //TODO pref to start w/ tool on?
                ToolSpeedPower = Project.CAM_Preferences.ToolSpeedPowerMax //TODOD?
            };

            Point startPoint = new Point(Project.CAM_Preferences.Start_X, Project.CAM_Preferences.Start_Y, Project.CAM_Preferences.Start_Z);
            
            //Move to start position
            var moveToStart = new LinearSegment()
            {
                Start = startPoint,
                End = new Point(Project.CAM_Preferences.Start_X, Project.CAM_Preferences.Start_Y, Project.CAM_Preferences.Start_Z),
                Settings = GetRapidSettings(startSettings)
            };

            //Move to first segments start
            var rapidToFirstSegment = new LinearSegment()
            {
                Start = moveToStart.End,
                End = new Point(segments.First().Start.X, segments.First().Start.Y, Project.CAM_Preferences.Rapid_Z_Level),
                Settings = GetRapidSettings(startSettings)
            };

            //Plunge to working Z
            var plungeToDepth = new LinearSegment()
            {
                Start = rapidToFirstSegment.End,
                End = segments.First().Start,
                Settings = GetPlungeSettings(startSettings)
            };

            //Insert into segment list
            if (Project.CAM_Preferences.InitializeAtStartPoisiton)
            {
                segments.InsertRange(0, new[] { moveToStart, rapidToFirstSegment, plungeToDepth });
            }
            else
            {
                segments.InsertRange(0, new[] {  rapidToFirstSegment, plungeToDepth });
            }
            
        }
        private void LinkWithRapids(List<Segment> segments)
        {
            if (!Segments.Any()) //First segments in list, no rapid to add
            {
                RapidToStart(segments);
                return;
            }


            Point lastPoint = Segments.Last().End;
            SegmentCAMSettings lastSettings = Segments.Last().Settings;

            //Lift from last segment ending
            var liftFromLast = new LinearSegment()
            {
                Start = lastPoint,
                End = new Point(lastPoint.X, lastPoint.Y,Project.CAM_Preferences.Rapid_Z_Level),
                Settings = GetRapidSettings(lastSettings)
            };

            //Move to current segments start
            var rapidFromLast = new LinearSegment()
            {
                Start = liftFromLast.End,
                End = new Point(segments.First().Start.X, segments.First().Start.Y, Project.CAM_Preferences.Rapid_Z_Level),
                Settings = GetRapidSettings(lastSettings)
            };

            var plungeToDepth = new LinearSegment()
            {
                Start = rapidFromLast.End,
                End = segments.First().Start,
                Settings = GetPlungeSettings(lastSettings)
            };

            //Insert into segment list
            segments.InsertRange(0, new[] { liftFromLast, rapidFromLast, plungeToDepth });
        }

        private void LeadInOut(List<Segment> segments)
        {
            //TODO      
            return;
        }

        private void OffsetSegements(List<Segment> segments, CutSide cutSide, bool blockOrderCW, double offsetDistance)
        {
            //Outside is +, Inside is -
            var signedOffset = cutSide == CutSide.Outside ? offsetDistance : -offsetDistance;

            var linearOffset = signedOffset;
            //For LinearSegments offset sign depends on block order only (CW/CCW)
            //  Outside (offset >0), CW block order -> offset left (sign +)
            //  Outside (offset >0), CCW block order -> offset right (sign -) (FLIP)
            //  Inside (offset <0), CW block order -> offset right (sign -)
            //  Inside (offset <0), CCW block order -> offset left (sign +) (FLIP)

            if (!blockOrderCW)
                linearOffset = -linearOffset;

            //For ArcSegments, offset sign depends on block order (CW/CCW) AND arc sense (CW/CCW) - XOR'd
            //Add/sub offset to radius
            //  Outside (offset >0), CW block order, CW arc -> offset + (sign +)
            //  Outside (offset >0), CW block order, CCW arc -> offset - (sign -) (FLIP)
            //  Outside (offset >0), CCW block order, CCW arc -> offset + (sign +)
            //  Outside (offset >0), CCW block order, CW arc -> offset - (sign -) (FLIP)

            //  Inside (offset <0), CW block order, CW arc -> offset - (sign -)
            //  Inside (offset <0), CW block order, CCW arc -> offset + (sign +) (FLIP)
            //  Inside (offset <0), CCW block order, CCW arc -> offset - (sign -)
            //  Inside (offset <0), CCW block order, CW arc -> offset + (sign +) (FLIP)

            //Offsetting
            foreach (var segment in segments)
            {
                switch (segment)
                {
                    case LinearSegment l:
                        segment.Offset(linearOffset);
                        break;
                    case ArcSegment a:
                        segment.Offset(blockOrderCW ^ a.CW ? -signedOffset : signedOffset);
                        break;
                    default:
                        break;
                }
            }

            //Trim overlapped segments
            for (int i = segments.Count - 1; i >= 0; i--)
            {
                var behindIndex = (i - 1) < 0 ? segments.Count - 1 : i - 1;
                var aheadIndex = i;
                var b = segments[behindIndex];
                var a = segments[aheadIndex];
                if(new Vector(b.End,a.Start).Magnitude <= Project.CAM_Preferences.MachineTolerance)
                    continue;
                
                var p = Segment.Intersection(a, b);
                if (p == null)
                    continue;

                b.End = p;
                a.Start = p;
            }

            //Else, insert arc to bridge the gap

            //Exted any open portions of the loop need tied with arcs
            //Loop backwards so we can insert along the way, without affecting the loop
            for (int i = segments.Count - 1; i >= 0; i--)
            {
                var behindIndex = (i-1) < 0? segments.Count - 1: i-1;
                var aheadIndex = i;
                var b = segments[behindIndex];
                var a = segments[aheadIndex];

                segments.InsertRange(aheadIndex, ArcsBetween(a, b, blockOrderCW));
            }




        }

        private List<ArcSegment> ArcsBetween(Segment ahead, Segment behind, bool CW)
        {
            var arcs = new List<ArcSegment>();
            var dist = (behind.End - ahead.Start).Magnitude;

            //Check start point of i and end point of i-1 
            // OR
            //If the dist is less than the sig figs for out put
            if (dist <= 10*Math.Pow(10,-Project.CAM_Preferences.RoundingDigits))
                return arcs;


            var startAngle = new Vector(behind.EndAngle);
            var endAngle = new Vector(ahead.StartAngle);

            var theta = endAngle.AngleBetween(startAngle);
            var b = Math.Abs(Math.Tan(theta / 4)) *(CW ? -1: 1);

            arcs = ArcSegment.BuildArcSegment(behind.End, ahead.Start, b);

            arcs.ForEach(item => item.Settings = behind.Settings);

            return arcs;

        }

        private void MultiPassSegments(List<Segment> segments,int passes, double passOffset, bool isClosedLoop)
        {
            var previousPass = segments.Clone(); //Copy of segments to reclone and reverse as needed

            //Passes/stepdown - open loop reverse for second pass
            for (int p = 1; p < passes; p++) //First pass exists as provided segments list
            {
                var passSegments = previousPass.Clone();

                if (!isClosedLoop && p % 2 != 0) //Open loop odd passes reverse direction
                    passSegments.ReverseSegements();

                passSegments.ShiftSegments(0, 0, passOffset); //Shift this pass down by offset for the pass

                //Insert transtion segment
                //Linear tool off, as rapid
                var transitionSegment = new LinearSegment()
                {
                    Start = previousPass.Last().End,
                    End = passSegments.First().Start,
                    Settings = GetPlungeSettings(passSegments.First().Settings)
                };
                passSegments.Insert(0, transitionSegment);

                segments.AddRange(passSegments);
                previousPass = passSegments;
            }

        }


    }
}

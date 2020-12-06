using BasicCAM.Geometry;
using BasicCAM.Interpreter;
using BasicCAM.Preferences;
using BasicCAM.Segments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BasicCAM
{
    [Serializable]
    public class BasicCAMProject
    {
        /// <summary>
        /// Generates 2D CAM (GCode) for given file and preferences.
        /// </summary>


        public string Title { get; set; }
        public ICAM_Preferences CAM_Preferences { get; set; }
        public IDocument_Preferences Document_Preferences { get; set; }
        public IMachine_Preferences Machine_Preferences { get; set; }
        public IGCode_Preferences GCode_Preferences { get; set; }
        public List<SegmentBlock> BlockList { get; set; } = new List<SegmentBlock>();

        public List<string> CAMErrors = new List<string>();
     
        //CAM solution for provided DXF file
        public BasicCAMProject(IGCode_Preferences gCode_Preferences,
                ICAM_Preferences  cAM_Preferences,
                IDocument_Preferences  document_Preferences,
                IMachine_Preferences  machine_Preferences)
        {

            GCode_Preferences = gCode_Preferences;
            CAM_Preferences = cAM_Preferences;
            Document_Preferences = document_Preferences;
            Machine_Preferences = machine_Preferences;
        }

        public void AutoCenter()
        {
            //TODO Find feature/motion center
            //Machine_Preferences.X_Centering_Offset= Machine_Preferences.WorkArea_X / 2 - Document.Center.x - Machine_Preferences.Tool_X_Offset;
            //Machine_Preferences.X_Centering_Offset = Machine_Preferences.WorkArea_Y / 2 - Document.Center.y - Machine_Preferences.Tool_Y_Offset;

            Machine_Preferences.X_Centering_Offset = Machine_Preferences.WorkArea_X / 2  - Machine_Preferences.Tool_X_Offset;
            Machine_Preferences.X_Centering_Offset = Machine_Preferences.WorkArea_Y / 2 - Machine_Preferences.Tool_Y_Offset;

        }

        ///// <summary>
        ///// Loads document depedning on extension.
        ///// Document will contain DocumentDefinedMotion
        ///// </summary>
        ///// <param name="fileName">Filename</param>
        //private void VerifyDocument()
        //{
        //    CAMErrors.Clear();

        //    //Check Document is with machine bounds, taking into account tool offset and reachable limits
        //    if (MachineableHeight! > Document.Heigth && MachineableWidth! > Document.Width)
        //        CAMErrors.Add("Document does not fit within machine limits");
        //}

        public void FindBlocks(IDocumentInterpreter document) {
            Point startingPoint = new Point(CAM_Preferences.Start_X, CAM_Preferences.Start_Y, CAM_Preferences.Start_Z);
            double chainTolerance = CAM_Preferences.MachineTolerance;

            //Features.FeatureList.Clear();

            var block = new SegmentBlock()
            {
                Name = "Document Feature"
            };

            List<Segment> remainingSegments = new List<Segment>(document.Segments);

            while (remainingSegments.Any())
            {
                var next = remainingSegments
                    .OrderBy(m => Math.Min(m.Start.To(startingPoint).Magnitude, m.End.To(startingPoint).Magnitude))
                    .First();

                var distance_start = next.Start.To(startingPoint).Magnitude;
                var distance_end = next.End.To(startingPoint).Magnitude;

                if(next.Length < CAM_Preferences.MachineTolerance)
                {
                    remainingSegments.Remove(next);
                    continue;
                }

                if (distance_start > distance_end)
                    next.Reverse();

                startingPoint = next.End;

                //Nearest outside of chianing tolerance, start a new chain.
                if (distance_start > chainTolerance && distance_end > chainTolerance && block.Segments.Any())
                {
                    BlockList.Add(block);

                    block = new SegmentBlock()
                    {
                        Name = "Block"
                    };
                }

                block.Segments.Add(next);
                remainingSegments.Remove(next);
            }

            if (block.Segments.Any())
                BlockList.Add(block);
        }

        public void ReorderBlockNearest()
        {
            //TODO
        }
        public void ReorderBlockXY()
        {
            var newOrderedList = BlockList
                .OrderBy(b => b.XMax)
                .OrderBy(b => b.YMax)
                .ToList();

            BlockList.Clear();
            BlockList.AddRange(newOrderedList);
        }

        /// <summary>
        /// Loads document into project as list of interpreted blocks
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool LoadDocument(IDocumentInterpreter document)
        {
            FindBlocks(document);
            ReorderBlockXY();
            return true;
        }

        /// <summary>
        /// Generates a bounding motion with pauses at corners, to align material. No power.
        /// </summary>
        //public void GenerateBoundingFile()
        //{
        //    BoundingFile = Path.GetTempFileName();

        //    using (GCodeStreamWriter gs =
        //      new GCodeStreamWriter(BoundingFile, CAM_Preferences.SignificantFigures))
        //    {
        //        GCodeStatementBuilder sBuilder = new GCodeStatementBuilder(gs, CAM_Preferences)
        //        {
        //            X_Offset = CAM_Preferences.AutoCenter ? X_Centering_Offset : 0.0,
        //            Y_Offset = CAM_Preferences.AutoCenter ? Y_Centering_Offset : 0.0,
        //            Scale_Factor = CAM_Preferences.ScaleFactor
        //        };
        //        var BoxMotions = new List<Feature>();

        //        BoxMotions.Add(new LinearFeature()
        //        {
        //            StartPoint = new Point(Document.Center.x - Document.Width/2, Document.Center.y + Document.Heigth/ 2),
        //            EndPoint = new Point(Document.Center.x + Document.Width / 2, Document.Center.y + Document.Heigth / 2),
        //        });

        //        BoxMotions.Add(new LinearFeature()
        //        {
        //            StartPoint = new Point(Document.Center.x + Document.Width / 2, Document.Center.y + Document.Heigth / 2),
        //            EndPoint = new Point(Document.Center.x + Document.Width / 2, Document.Center.y - Document.Heigth / 2),
        //        });

        //        BoxMotions.Add(new LinearFeature()
        //        {
        //            StartPoint = new Point(Document.Center.x + Document.Width / 2, Document.Center.y - Document.Heigth / 2),
        //            EndPoint = new Point(Document.Center.x - Document.Width / 2, Document.Center.y - Document.Heigth / 2),
        //        });

        //        BoxMotions.Add(new LinearFeature()
        //        {
        //            StartPoint = new Point(Document.Center.x - Document.Width / 2, Document.Center.y - Document.Heigth / 2),
        //            EndPoint = new Point(Document.Center.x - Document.Width / 2, Document.Center.y + Document.Heigth / 2),
        //        });

        //        //GCODE HEADER WRITING
        //        sBuilder.StartMotionChain(BoxMotions[0]);
        //        foreach (var Motion in BoxMotions)
        //        {
        //            //WRITE CHAIN
        //            sBuilder.Motion(Motion);
        //        }
        //        sBuilder.StopMotionChain();
        //    }
        //}
    }


}

using DXFEntityReader;
using System;
using System.Collections.Generic;
using System.Text;
using BasicCAM.Geometry;
using System.IO;
using System.Text.RegularExpressions;
using BasicCAM.Preferences;
using System.Dynamic;
using BasicCAM.Writer;
using BasicCAM.Motions;

namespace BasicCAM.Interpreter
{
    public class GCode_Interpreter : DocumentInterpreterBase, IDocumentInterpreter
    {

        //TODO deal with G91 (relative positions)
        //TODO U, V, W, 

        public double DocumentScale { get; set; } = 1;
        private IGCode_Preferences _gCode {get;set;}
        //Class to wrap GCode Document and create 'standardized' shapes list.
        public GCode_Interpreter(IGCode_Preferences gCode_Preferences)
        {
            _gCode = gCode_Preferences;
        }
        public void LoadStream(StreamReader stream)
        {
          
            string? line = stream.ReadLine();
            int line_num = 1;

            Motion CurrentFeature = new Motion()
            {
                StartPoint = new Point(),
                EndPoint = new Point()
            };
            GCodeLine gCodeStatement;
            GCodeReader parser = new GCodeReader() ;
            while (!stream.EndOfStream && line !=null )
            {
                try
                {
                    gCodeStatement = parser.BuildStatement(line);
                    switch (gCodeStatement.MotionWord.Type)
                    {
                        case WORD_TYPE.G:
                            if (gCodeStatement.MotionWord.Value == _gCode.LinearRapid.Value 
                                || gCodeStatement.MotionWord.Value == _gCode.Linear.Value)
                                CurrentFeature = BuildLinearFeature(gCodeStatement, CurrentFeature);

                            if (gCodeStatement.MotionWord.Value == _gCode.ArcCW.Value 
                                || gCodeStatement.MotionWord.Value == _gCode.ArcCCW.Value)
                                CurrentFeature = BuildArcFeature(gCodeStatement, CurrentFeature);

                            if (gCodeStatement.MotionWord.Value == _gCode.Rehome.Value)
                                CurrentFeature = BuildBaseFeature(gCodeStatement, CurrentFeature);

                            break;
                        case WORD_TYPE.M:
                            CurrentFeature = BuildBaseFeature(gCodeStatement, CurrentFeature);
                            break;
                        default:
                            throw new Exception($"Unknown Code Type {gCodeStatement.MotionWord.Type}, on line {line_num}");
                    }
                    Motions.Add(CurrentFeature);
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
                finally{
                    line = stream.ReadLine(); //LOOP
                    line_num++;
                }
            }
        }
        public Motion BuildBaseFeature(GCodeLine gCodeStatement, Motion lastFeature)
        {
            Motion nextFeature = new Motion();

            nextFeature.StartPoint = lastFeature.EndPoint;
            nextFeature.EndPoint = lastFeature.EndPoint;
            nextFeature.State = lastFeature.State;

            //SET END TO THE SAME AS THE LAST KNOWN POSITION AND REPLACE WHAT IS GIVEN AS VAR
            //IN THIS MOTION. IF VAR IS OMITTED IT'S ASSUMED TO HAVE NOT CHANGED
            if (gCodeStatement.TryGetWord(WORD_TYPE.X, out GCodeWord x))
                nextFeature.EndPoint.x = x.WordValue;

            if (gCodeStatement.TryGetWord(WORD_TYPE.X, out GCodeWord y))
                nextFeature.EndPoint.y = y.WordValue;

            if (gCodeStatement.TryGetWord(WORD_TYPE.X, out GCodeWord z))
                nextFeature.EndPoint.z = z.WordValue;

            return nextFeature;
        }
        public LinearMotion BuildLinearFeature(GCodeLine gCodeStatement, Motion lastFeature)
        {
            var linearFeature = new LinearMotion();

            linearFeature.StartPoint = lastFeature.EndPoint;
            linearFeature.EndPoint= lastFeature.EndPoint;
            linearFeature.State = lastFeature.State;

            if (gCodeStatement.TryGetWord(WORD_TYPE.X, out GCodeWord x))
                linearFeature.EndPoint.x = x.WordValue;

            if (gCodeStatement.TryGetWord(WORD_TYPE.X, out GCodeWord y))
                linearFeature.EndPoint.y = y.WordValue;

            if (gCodeStatement.TryGetWord(WORD_TYPE.X, out GCodeWord z))
                linearFeature.EndPoint.z = z.WordValue;

            return linearFeature;
        }

        public ArcMotion BuildArcFeature(GCodeLine gCodeStatement, Motion lastFeature)
        {
            var arcFeature = new ArcMotion()
            {
                CW = gCodeStatement.MotionWord.Value == _gCode.ArcCW.Value ? true : false,
            };

            arcFeature.StartPoint = lastFeature.EndPoint;
            arcFeature.EndPoint = lastFeature.EndPoint;
            arcFeature.State = lastFeature.State;

            if (gCodeStatement.TryGetWord(WORD_TYPE.X, out GCodeWord x))
                arcFeature.EndPoint.x = x.WordValue;

            if (gCodeStatement.TryGetWord(WORD_TYPE.X, out GCodeWord y))
                arcFeature.EndPoint.y = y.WordValue;

            if (gCodeStatement.TryGetWord(WORD_TYPE.X, out GCodeWord z))
                arcFeature.EndPoint.z = z.WordValue;

            //TWO arc types, IJK and R. 

            //IJK is offset from start point to center point
            //R is radius
            if (gCodeStatement.TryGetWord(WORD_TYPE.R, out GCodeWord r)) /// R arc
            {
                arcFeature.Radius = r.Value;

                //Locate center
                Point mid = Point.MidPoint(arcFeature.StartPoint, arcFeature.EndPoint);
                double midPointDistance = Point.DistanceBetween(arcFeature.StartPoint, arcFeature.EndPoint) / 2;
                // + angle is CW
                // - angle is CCW
                double angleToCenter = Math.Acos(midPointDistance / arcFeature.Radius) * (arcFeature.CW ? 1 : -1);
                arcFeature.Center = arcFeature.StartPoint
                    .VectorTo(arcFeature.EndPoint)
                    .Normalize()
                    .Rotate2D(angleToCenter)
                    .Scale(arcFeature.Radius)
                    .Add(arcFeature.StartPoint);
            }
            else /// IJK arc
            {
                gCodeStatement.TryGetWord(WORD_TYPE.I, out GCodeWord i);
                gCodeStatement.TryGetWord(WORD_TYPE.J, out GCodeWord j);
                gCodeStatement.TryGetWord(WORD_TYPE.K, out GCodeWord k);
                
                arcFeature.Center = arcFeature.StartPoint.Add(new Point(i.Value,j.Value, k.Value));
                arcFeature.Radius = arcFeature.StartPoint.VectorTo(arcFeature.Center).Magnitude;
            }

            return arcFeature;
        }
        public override void LoadDocument(string filePath,double scale = 1)
        {
            DocumentScale = scale;
            this.FilePath = filePath;
            if (File.Exists(filePath)) 
            {
                using (var file = File.Open(filePath,FileMode.Open,FileAccess.Read,FileShare.Read))
                {
                    LoadStream(new StreamReader(file));
                }
            }
        }

        
    }
}

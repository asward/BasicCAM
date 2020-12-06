using BasicCAM.Motions;
using BasicCAM.Geometry;
using BasicCAM.Preferences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BasicCAM.Interpreter
{
    public class GCodeReader
    {
        private const string block_delete_pattern = @"(^\/)";
        private const string comment_pattern = @"\([^)]*\)"; //TODO unmatched parentheses comment 
        //private const string line_num_pattern = @"(^N[0-9]*.)";
        private const string semicolons_pattern = @"(\;)";
        private const string checksum_pattern = @"(\*.*)";

        private const string command_pattern = @"[GM]\w*";
        private const string parameter_pattern = @"([A-Za-z][+-]?\d*\.?\d*)";
        private const string parameter_code_pattern = @"([A-Za-z])";
        private const string parameter_value_pattern = @"([+-]?\d*\.?\d*)";


        public GCodeLine BuildStatement(string line)
        {
            var block_delete_match = Regex.Match(line, block_delete_pattern);

            if (!block_delete_match.Success && command_match.Success)
            {
                var command_match = Regex.Match(line, command_pattern);
                if (command_match.Success)
                    var gCodeStatement = new GCodeLine(command_match.Value);

                BuildParameters(gCodeStatement, line);

                var comment_match = Regex.Match(line, comment_pattern);
                if (comment_match.Success)
                    gCodeStatement.Comments = comment_match.Value;

                return gCodeStatement;
            }

            return null; 
        }

      
        private void BuildParameters(GCodeLine gCodeStatement, string line)
        {
            line = Regex.Replace(line, comment_pattern, ""); //REMOVE COMMENTS
            //line = Regex.Replace(line, line_num_pattern, ""); //REMOVE LINE NUMBERS
            line = Regex.Replace(line, semicolons_pattern, ""); //REMOVE LINE TERMINATORS
            line = Regex.Replace(line, checksum_pattern, ""); //REMOVE CHECKSUM

            line = Regex.Replace(line, command_pattern, ""); //REMOVE COMMAND

            var parameters = Regex.Match(line, parameter_pattern);
            foreach (Group parameter in parameters.Groups)
            {
                double result = 0;
                var code = Regex.Match(parameter.Value, parameter_code_pattern);
                var value = Regex.Match(parameter.Value, parameter_value_pattern);
                if (value.Success)
                    double.TryParse(value.Value, out result);
                if (code.Success && value.Success)
                    Parameters.Add((code.Value, result));
            }
        }
        public GCodeReader Parameter(string parameter, double value)
        {
            this.Parameters.Add((parameter, value));
            return this;
        }
        public GCodeReader Parameter(GCodeWord gCodeParmeter)
        {
            this.Parameters.Add(gCodeParmeter);
            return this;
        }
        public GCodeReader Comment(string comment)
        {
            this.Comments = comment;
            return this;
        }

        public string ToString(IGCode_Preferences gCode_Preferences, int significantFigures, double scaleFactor, double x_offset, double y_offset)
        {
            string output = Command.ToString(gCode_Preferences);
            foreach(var parameter in Parameters)
            {
                output += parameter.ToString(gCode_Preferences);
            }

            if(!String.IsNullOrEmpty(Comments))
                output += $"{gCode_Preferences.StartCommentDelimeter}{Comments}{gCode_Preferences.EndCommentDelimeter}" ;

            return output;
        }
        string ParameterToString(string parameter, double value, int significantFigures)
        {
            double tol_val = RoundToTolernace(value, significantFigures);
            string val_string = ApplySignificantDigits(tol_val, significantFigures);

            return $"{parameter}{val_string}";
        }
        private string ApplySignificantDigits(double d, int sig_digits)
        {
            return ((Decimal)Double.Parse(d.ToString($"G{sig_digits}"))).ToString();
        }


        private double RoundToTolernace(double d, int tol_digits)
        {
            return Math.Round(d, tol_digits);
        }
    }

}

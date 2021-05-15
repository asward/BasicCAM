using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BasicCAM.Core.GCode
{
    [Serializable]
    public class GCodeWord
    {
        public static implicit operator GCodeWord(string s) => new GCodeWord(s);
        public static implicit operator GCodeWord(ValueTuple<string, double> sv) => new GCodeWord(sv.Item1, sv.Item2);
        private const string parameter_code_pattern = @"([A-Za-z])";
        private const string parameter_value_pattern = @"([+-]?\d+\.?\d*)";
        private readonly string wordString  = null;
        private readonly string wordType  = "";
        private readonly double wordValue  = 0.0 ;
        public GCodeWord(string wordString)
        {
            this.wordString = wordString;
        }
        public GCodeWord(string wordType, double wordValue)
        {
            this.wordType = wordType;
            this.wordValue = wordValue;
        }
        public double Value
        {
            get
            {
                if (null == wordString)
                    return wordValue;

                return double.Parse(Regex.Match(wordString, parameter_value_pattern).Value);
            }
        }
        public string Type
        {
            get
            {
                if (null == wordString)
                    return wordType;

                return Regex.Match(wordString, parameter_code_pattern).Value;
            }
        }


        public string ToString(int precision = 5)
        {
            double tol_val = Math.Round(Value, precision);
            string val_string = ((Decimal)Double.Parse(tol_val.ToString($"G"))).ToString(); 
            
            return $"{Type}{val_string}";
        }
    }
}

using BasicCAM.Preferences;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BasicCAM.GCode
{
    [Serializable]
    public class GCodeLine
    {
        public List<GCodeWord> Words { get; set; } = new List<GCodeWord>();
        public string Comments { get;  set; }
        public GCodeLine(GCodeWord function)
        {
            Words.Add(function);
        }
        public static GCodeLine FromWord(GCodeWord word)
        {
            return new GCodeLine(word);
        }
        public GCodeLine Word(string word, double value)
        {
            this.Words.Add((word, value));
            return this;
        }
        public GCodeLine Word(GCodeWord gCodeWord)
        {
            this.Words.Add(gCodeWord);
            return this;
        }
        public GCodeLine Comment(string comment)
        {
            this.Comments = comment;
            return this;
        }

        internal bool TryGetWord(string wordType, out GCodeWord gCodeWord)
        {
            gCodeWord = null;
            var matches = Words.Where(p => String.Equals(p.Type, wordType, StringComparison.OrdinalIgnoreCase));
            if (matches.Any())
            {
                gCodeWord = matches.First();
                return true;
            }

            return false;
        }
    }
}

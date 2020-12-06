using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAM.Preferences
{
    public enum CutSide
    {
        Outside,
        Inside,
        Center
    }
    [Serializable]
    public class BlockPreferences
    {
        private double _powerFactor = 1;
        public double PowerFactor {
            get 
            { 
                return _powerFactor; 
            } 
            set {
                _powerFactor = value;
                 
                if (_powerFactor > 1)
                    _powerFactor = 1;

                if(_powerFactor < 0)
                    _powerFactor = 0;
            }
        }

        private double _feedRateFactor = 1;
        public double FeedRateFactor
        {
            get
            {
                return _feedRateFactor;
            }
            set
            {
                _feedRateFactor = value;

                if (_feedRateFactor > 1)
                    _feedRateFactor = 1;

                if (_feedRateFactor < 0)
                    _feedRateFactor = 0;
            }
        }
        public int Passes { get; set; } = 1;
        public double Passoffset { get; set; } = 0.0;
        public bool InsertLeadIn { get; set; } = false;
        public CutSide CutSide { get; set; } = CutSide.Center;
        public bool Rapid { get; set; } = false;
        public bool ToolOn { get; set; } = true;
        
    }
}

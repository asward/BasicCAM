using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BasicCAM.Core.Preferences
{
    public class Document_Preferences : IDocument_Preferences
    {
        [DisplayName("Inside Layer")]
        [Description("Identifier for Inside cut layers (case insensitive)")]
        public virtual string InsideLayer { get; set; } = "INSIDE";

        [DisplayName("Outside Layer")]
        [Description("Identifier for Outside cut layers (case insensitive)")]
        public virtual string OutsideLayer { get; set; } = "OUTSIDE";

        [DisplayName("Scale Factor")]
        [Description("Scaling to apply to document")]
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Value must be between {1} and {2}")]
        public virtual double ScaleFactor { get; set; } = 1;

        [DisplayName("Feature Tolerance")]
        [Description("Maximum gap size between feature segments")]
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Value must be between {1} and {2}")]
        public virtual double FeatureTolerance { get; set; } = 0.0001;
    }


    public interface IDocument_Preferences
    {
        public string InsideLayer { get; set; }
        public string OutsideLayer { get; set; }
        public double ScaleFactor { get; set; }
        double FeatureTolerance { get; set; }
    }
}

using BasicCAM.Core.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using System.Text;

namespace BasicCAM.Core.Preferences
{
    public enum CutSide
    {
        Outside,
        Inside,
        Center
    }
    [Serializable]
    public class FeatureSettings
    {

        [DisplayName("Feature Name")]
        [Description("Identifier for the feature")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$",
         ErrorMessage = "Characters are not allowed.")]
        public string Name { get; set; } = "Feature";

        [DisplayName("Passes")]
        [Description("Number of passes for this feature")]
        [Range(1,255,ErrorMessage = "Must be between 1 and 255")]
        public int Passes { get; set; } = 1;


        [DisplayName("Z Step Over")]
        [Description("Z delta for successive passes.")]
        [Range(0, 255, ErrorMessage = "Must be between 0 and 255")]
        public double ZStepOver { get; set; } = 0.0;


        [DisplayName("Insert Lead-In")]
        [Description("Add arc lead-in to feature.")]
        [DisableDisplay]
        public bool InsertLeadIn { get; set; } = false;

        [DisplayName("Insert Lead-Out")]
        [Description("Add arc lead-out to feature.")]
        [DisableDisplay]
        public bool InsertLeadOut { get; set; } = false;

        [DisplayName("Cut Side")]
        [Description("The side of the feature the tool will pass on")]
        public CutSide CutSide { get; set; } = CutSide.Inside;

        [DisplayName("Feedrate Factor")]
        [Description("Multiplicative factor for feedrate")]
        [Range(0, 1, ErrorMessage = "Must be between 0 and 1")]
        public double FeedrateFactor { get; set; } = 1;

        [DisplayName("Tool Speed/Power Factor")]
        [Description("Multiplicative factor for tool speed/power")]
        [Range(0, 1, ErrorMessage = "Must be between 0 and 1")]
        public double ToolSpeedPowerFactor { get; set; } = 1;

    }
}

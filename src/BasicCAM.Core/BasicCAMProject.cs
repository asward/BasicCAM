using BasicCAM.Core.Features;
using BasicCAM.Core.Geometry;

using BasicCAM.Core.Preferences;
using BasicCAM.Core.Segments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace BasicCAM.Core
{

    /// <summary>
    /// Settings and features to be machined
    /// </summary>
    [Serializable]
    public class BasicCAMProject
    {
        public string Name { get; set; } = "New Project";
        public string Guid { get; set; } = (new Guid()).ToString();
        public DateTime SaveTime { get; set; } = DateTime.UtcNow;

        public ICAM_Preferences CAM_Preferences { get; set; }
        public IDocument_Preferences Document_Preferences { get; set; }
        public IMachine_Preferences Machine_Preferences { get; set; }
        public IGCode_Preferences GCode_Preferences { get; set; }
        public ITool_Preferences Tool_Preferences { get; set; }

        private  List<Feature> _features = new List<Feature>();
        public List<Segment> Segments
        {
            get {
                return _features.SelectMany(f => f.Segments).ToList();
            }
        }
        public IReadOnlyCollection<Feature> Features => _features;

        public List<string> CAMErrors = new List<string>();
     
       
        public BasicCAMProject(IGCode_Preferences gCode_Preferences,
                ICAM_Preferences  cAM_Preferences,
                IDocument_Preferences  document_Preferences,
                IMachine_Preferences  machine_Preferences,
                ITool_Preferences tool_Preferences)
        {

            GCode_Preferences = gCode_Preferences;
            CAM_Preferences = cAM_Preferences;
            Document_Preferences = document_Preferences;
            Machine_Preferences = machine_Preferences;
            Tool_Preferences = tool_Preferences;
        }

        public void ReorderFeaturesXY()
        {
            _features
                .OrderBy(b => b.XMax)
                .OrderBy(b => b.YMax)
                .ToList();
        }

        /// <summary>
        /// Loads document into project as features
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public void AddFeatures(IEnumerable<Feature> features)
        {
            _features.AddRange(features);
        }
        public void RemoveFeatures(IEnumerable<Feature> features)
        {
            foreach(var feature in features)
            {
                _features.Remove(feature);
            }
        }
    }


}

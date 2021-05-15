using BasicCAM.Core;
using BasicCAM.Core.Preferences;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicCAM.Tests
{
    public static class Seed
    {
        internal static BasicCAMProject SeedProject()
        {
            var GCode_Preferences= new Mock<IGCode_Preferences>();
            var CAM_Preferences = new Mock<ICAM_Preferences>();
            var Document_Preferences = new Mock<IDocument_Preferences>();
            var Machine_Preferences = new Mock<IMachine_Preferences>();
            var Tool_Preferences = new Mock<ITool_Preferences>();
            return new BasicCAMProject(GCode_Preferences.Object,
                                       CAM_Preferences.Object,
                                       Document_Preferences.Object,
                                       Machine_Preferences.Object,
                                       Tool_Preferences.Object);
        }
        internal static string GetFullPathToFile(string pathRelativeUnitTestingFile)
        {
            string folderProjectLevel = GetPathToCurrentUnitTestProject();
            string final = System.IO.Path.Combine(folderProjectLevel, pathRelativeUnitTestingFile);
            return final;
        }
        /// <summary>
        /// Get the path to the current unit testing project.
        /// </summary>
        /// <returns></returns>
        private static string GetPathToCurrentUnitTestProject()
        {
            string pathAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string folderAssembly = System.IO.Path.GetDirectoryName(pathAssembly);
            if (folderAssembly.EndsWith("\\") == false) folderAssembly = folderAssembly + "\\";
            string folderProjectLevel = System.IO.Path.GetFullPath(folderAssembly + "..\\..\\");
            return folderProjectLevel;
        }
    }
}

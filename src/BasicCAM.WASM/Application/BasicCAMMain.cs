using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using BasicCAM.Core;
using BasicCAM.Core.Solutions;
using BasicCAM.Core.Preferences;

using Microsoft.Extensions.Logging;
using Blazored.LocalStorage;

using BasicCAM.Core.GCode;
using System.Text.Json;
using BasicCAM.Core.Features;

namespace BasicCAM.WASM.Application
{
    public class BasicCAMMain
    {
        private readonly ILogger<BasicCAMMain> logger;
        private readonly ILocalStorageService localStorage;
        public BasicCAMMain(ILogger<BasicCAMMain> _logger, ILocalStorageService _localStorage, IUserInputService userInputService)
        {
            logger = _logger;
            localStorage = _localStorage;
            UserInputService = userInputService;
            InitializeData();
        }
        
        public IUserInputService UserInputService { get; }
        public BasicCAMProject BasicCAMProject { get; set; }
        public CAMSolution BasicCAMSolution { get; set; }
        public GCodeOutput GCodeOutput { get; set; }
        public bool UnsavedChanges { get; private set; } = true;

        private bool _hasChanges = false;
        public bool HasChanges { get
            {
                return _hasChanges;
            }
            private set
            {
                if(_hasChanges)
                    OnSolutionStateChange(new SolutionStateChangeEventArgs(value));

                _hasChanges = value;
            }
        }

        private void InitializeData()
        {

            BasicCAMProject = new BasicCAMProject(
                new GCode_Preferences(),
                new CAM_Preferences(),
                new Document_Preferences(),
                new Machine_Preferences(),
                new Tool_Preferences());

            BasicCAMSolution = new CAMSolution(BasicCAMProject);

            GCodeOutput = new GCodeOutput(BasicCAMProject.GCode_Preferences);
        }

        public void PreferenceChanged()
        {
            HasChanges = true;
            UnsavedChanges = true;
        }

        public async Task AddFile(byte[] fileBytes)
        {
            try
            {
                var featureBuilder = new DXFFeatureBuilder(BasicCAMProject.Document_Preferences);
                var features = await featureBuilder.FromBytes(fileBytes);

                BasicCAMProject.AddFeatures(features);

                logger.LogInformation($"Found {BasicCAMProject.Features.Count} features from {BasicCAMProject.Features.Sum(f=>f.Segments.Count)} segments");
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, "Failed to open file.");
            }

            await RedrawCAD();
            await Recalculate();
        }

        /// <summary>
        /// Clears current project and creates a new empty project
        /// </summary>
        public async Task NewProject()
        {

            logger.LogInformation("New Project.");

            if (!(await ContinueWithUnsavedChanges()))
                return;

            InitializeData();

            await RedrawCAD();
            await Recalculate();
        }

        

        /// <summary>
        /// Save project under new reference
        /// </summary>
        public async Task SaveProjectAs()
        {
            await Recalculate();

            BasicCAMProject.Guid = (new Guid()).ToString();
            
            await SaveProject();
        }

        /// <summary>
        /// Save project existing reference
        /// </summary>
        public async Task SaveProject()
        {
            await Recalculate();

            //Create a serialized file 
            using var ms = new MemoryStream();

            try
            {
                BasicCAMProject.SaveTime = DateTime.UtcNow;

                await JsonSerializer.SerializeAsync<BasicCAMProject>(ms, BasicCAMProject);

                await localStorage.SetItemAsync(BasicCAMProject.Guid, Encoding.ASCII.GetString(ms.ToArray()));

                UnsavedChanges = false;
            }
            catch (SerializationException e)
            {
                logger.LogError(e.Message, "Failed to serialize.");
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, "Error during saving.");
            }
        }

        public async Task LoadProject(byte[] projectBytes)
        {
            if (!(await ContinueWithUnsavedChanges()))
                return;

            try
            {
                BasicCAMProject = JsonSerializer.Deserialize<BasicCAMProject>(projectBytes);

                BasicCAMSolution = new CAMSolution(BasicCAMProject);

                GCodeOutput = new GCodeOutput(BasicCAMProject.GCode_Preferences);

                await Recalculate();
            }
            catch (Exception e)
            {
                await NewProject();
                logger.LogError(e.Message, "Failed to deserialize.");
            }
            await RedrawCAD();
            await Recalculate();
        }

        public async Task RedrawCAD()
        {
            OnCadDataChanged(new EventArgs());
        }
        /// <summary>
        /// Recalulate the soltuion
        /// </summary>
        /// <param name="obj"></param>
        public async Task Recalculate()
        {
            if (null == BasicCAMSolution)
                return;

            await BasicCAMSolution.SolveAsync();

            GCodeOutput
                .Reset()
                .AddSegments(BasicCAMSolution.SolutionSegments);

            HasChanges = false;

            OnSolutionDataChanged(new EventArgs());
        }

        /// <summary>
        /// Checks if there are unsaved changes, then prompts to continue with outsaving them
        /// </summary>
        /// <returns>Returns true if the calling operation should continue</returns>
        private async Task<bool> ContinueWithUnsavedChanges()
        {
            if (UnsavedChanges)
                return await UserInputService.VerifyAction("Unsaved Changes", "Project contains unsaved changes. Continue without saving?");

            return true;
        }


        public event EventHandler SolutionDataChanged;
        private void OnSolutionDataChanged(EventArgs e)
        {
            EventHandler handler = SolutionDataChanged;
            handler?.Invoke(this, e);
        }


        public event EventHandler CadDataChanged;
        private void OnCadDataChanged(EventArgs e)
        {
            EventHandler handler = CadDataChanged;
            handler?.Invoke(this, e);
        }


        public event SolutionStateChangeEventHandler SolutionStateChanged;
        private void OnSolutionStateChange(SolutionStateChangeEventArgs e)
        {
            SolutionStateChangeEventHandler handler = SolutionStateChanged;
            handler?.Invoke(this, e);
        }
        public delegate void SolutionStateChangeEventHandler(object sender, SolutionStateChangeEventArgs args);

        public class SolutionStateChangeEventArgs : EventArgs
        {
            public SolutionStateChangeEventArgs(bool hasChanges)
            {
                HasChanges = hasChanges;
            }

            public bool HasChanges { get; }
        }
    }
}

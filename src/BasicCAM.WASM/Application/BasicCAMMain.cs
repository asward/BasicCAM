using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasicCAM.Preferences;
using BasicCAM;
using BasicCAM.Interpreter;
using System.IO;
using Tewr.Blazor.FileReader;
using BlazorStrap;
using Microsoft.AspNetCore.Components;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Blazored.LocalStorage;
using System.Text;
using Blazored.Modal.Services;
using BasicCAM.GCode;
using System.Collections.ObjectModel;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using BasicCAM.Segments;
using BasicCAM.WASM.Serialization;
using System.Text.Json;
using BasicCAMWebAppWASM.Models.Preferences;

namespace BasicCAM.WASM.Application
{
    public class BasicCAMMain
    {
        private readonly IJSRuntime jsRuntime;

        enum Trigger
        {
        }

        enum State
        {
            Ready,
            Generating,
            PendingChanges,
        }

        public ILogger<BasicCAMMain> logger { get; set; }
        public BasicCAMMain(ILogger<BasicCAMMain> _logger, ILocalStorageService _localStorage, IJSRuntime jsRuntime)
        {
            logger = _logger;
            localStorage = _localStorage;
            this.jsRuntime = jsRuntime;
        }
        public ILocalStorageService localStorage { get; set; }
        public ICAM_Preferences CAM_Preferences { get; set; } = new CAM_Preferences();
        public IDocument_Preferences Document_Preferences { get; set; } = new Document_Preferences();
        public IGCode_Preferences GCode_Preferences { get; set; } = new GCode_Preferences();
        public IMachine_Preferences Machine_Preferences { get; set; } = new Machine_Preferences();

        public BasicCAMProject BasicCAMProject { get; set; }
        public BasicCAMSolution BasicCAMSolution { get; set; }

        private DXF_Interpreter DXF_Interpreter { get; set; }

        private int ProjectID { get; set; }
        private string ProjectName { get; set; }
        public bool UnsavedChanges { get; private set; }
        public List<string> GCode { get; set; } = new List<string>();

        public async Task AddFile(byte[] fileBytes)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(fileBytes, 0, fileBytes.Length);
                    stream.Seek(0, SeekOrigin.Begin);

                    DXF_Interpreter = new DXF_Interpreter();
                    await DXF_Interpreter.LoadStream(stream);

                    BasicCAMProject = new BasicCAMProject(GCode_Preferences, CAM_Preferences, Document_Preferences, Machine_Preferences);

                    BasicCAMProject.LoadDocument(DXF_Interpreter);

                    BasicCAMSolution = new BasicCAMSolution(BasicCAMProject);
                    BasicCAMSolution.Solve();

                    logger.LogInformation(BasicCAMSolution.Segments.Count.ToString());
                }
            }
            catch (Exception e)
            {
                NewProject();
                logger.LogError(e.Message, "Failed to open file.");
            }
        }

        /// <summary>
        /// Clears current project and creates a new empty project
        /// </summary>
        public void NewProject()
        {

            logger.LogInformation("New Project.");

            if (!ContinueWithUnsavedChanges())
                return;


            CAM_Preferences = new CAM_Preferences();
            Document_Preferences = new Document_Preferences();
            GCode_Preferences = new GCode_Preferences();
            Machine_Preferences = new Machine_Preferences();

            BasicCAMProject = new BasicCAMProject(
                GCode_Preferences,
                CAM_Preferences,
                Document_Preferences,
                Machine_Preferences);

            BasicCAMSolution = new BasicCAMSolution(BasicCAMProject);

            Refresh();
        }


        /// <summary>
        /// Save project under new reference
        /// </summary>
        public async Task SaveProjectAs()
        {
            //Open naming modal/menu
            ProjectName = ProjectName; //TODO
            await SaveProject();
        }

        /// <summary>
        /// Save project existing reference
        /// </summary>
        public async Task SaveProject()
        {
            if (String.IsNullOrEmpty(ProjectName))
                await SaveProjectAs();

            //Create a serialized file 
            using (var ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();

                try
                {
                    formatter.Serialize(ms, BasicCAMProject);

                    await localStorage.SetItemAsync(ProjectName, Encoding.ASCII.GetString(ms.ToArray()));

                    UnsavedChanges = false;
                    //TOTO Write to server/db

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
        }

        public async Task LoadProject()
        {
            if (!ContinueWithUnsavedChanges())
                return;


            string projectString = await localStorage.GetItemAsync<string>(ProjectName);
            byte[] projectBytes = ASCIIEncoding.ASCII.GetBytes(projectString);

            try
            {
                using (var ms = new MemoryStream(projectBytes))
                {

                    BinaryFormatter formatter = new BinaryFormatter();
                    BasicCAMProject = (BasicCAMProject)formatter.Deserialize(ms);
                    BasicCAMSolution = new BasicCAMSolution(BasicCAMProject);
                    Refresh();
                }
            }
            catch (Exception e)
            {
                NewProject();
                logger.LogError(e.Message, "Failed to deserialize.");
            }

            Refresh();
        }


        /// <summary>
        /// Refresh Solution and UI
        /// </summary>
        private void Refresh()
        {
            Reclaculate();

            Reload();
        }

        /// <summary>
        /// Reload the View from the current drawing solution
        /// </summary>
        private void Reload()
        {
            return;
        }

        /// <summary>
        /// Recalulate the soltuion
        /// </summary>
        /// <param name="obj"></param>
        public async void Reclaculate()
        {
            if (null == BasicCAMSolution)
                return;

            BasicCAMSolution.Solve();

            await WriteSolution();
            await DisplaySolution();
            OnSolutionDataChanged(new EventArgs());
        }

        /// <summary>
        /// Write soltuion GCode into text box and link to drawing
        /// </summary>
        /// <returns></returns>
        private async Task WriteSolution()
        {
            byte[] data;
            GCodeWriter gcw = new GCodeWriter(BasicCAMSolution);
            using (MemoryStream ms = new MemoryStream())
            {
                await gcw.WriteToStreamAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = await sr.ReadLineAsync();
                        logger.LogDebug(line);
                        GCode.Add(line);
                    }
                }
            }

        }

        /// <summary>
        /// Sends solution geometery to JS renderer
        /// </summary>
        /// <returns></returns>
        private async Task DisplaySolution()
        {
            //JsonSerializerSettings settings = new JsonSerializerSettings
            //{
            //    SerializationBinder = new KnownTypesBinder
            //    {
            //        KnownTypes = new List<Type> { typeof(ArcSegment), typeof(LinearSegment) }
            //    },
            //    TypeNameHandling = TypeNameHandling.Objects
            //};

            //var json = JsonConvert.SerializeObject(BasicCAMSolution.Segments);

            var options = new JsonSerializerOptions();
            options.Converters.Add(new SegmentSerializerConverter());
            string json = System.Text.Json.JsonSerializer.Serialize(BasicCAMSolution.Segments, options);

            await jsRuntime.InvokeVoidAsync("BasicCAMJS.plot", json);
        }

        /// <summary>
        /// Checks if there are unsaved changes, then prompts to continue with outsaving them
        /// </summary>
        /// <returns>Returns true if the calling operation should continue</returns>
        private bool ContinueWithUnsavedChanges()
        {
            return true;

        }


        public event EventHandler SolutionDataChanged;
        public virtual async void OnSolutionDataChanged(EventArgs e)
        {
            EventHandler handler = SolutionDataChanged;
            handler?.Invoke(this, e);
        }
    }
}

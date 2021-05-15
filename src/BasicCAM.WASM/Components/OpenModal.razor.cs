using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//using Tewr.Blazor.FileReader;

namespace BasicCAM.WASM.Components
{
    public class OpenModalResults
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public byte[] ContentBuffer { get; set; }

    }

    public partial class OpenModal
    {
        [CascadingParameter]
        BlazoredModalInstance BlazoredModal { get; set; }

        async Task OpenNewFile(InputFileChangeEventArgs e)
        {
            await BlazoredModal.Close(ModalResult.Ok<OpenModalResults>(await BuildModalResult(e.File)));
        }
        private async Task<OpenModalResults> BuildModalResult(IBrowserFile file)
        {
            var result = new OpenModalResults()
            {
                FileName = Path.GetFileNameWithoutExtension(file.Name),
                FileExtension = Path.GetExtension(file.Name),
            };

            result.ContentBuffer = new byte[file.Size];
            using (Stream stream = file.OpenReadStream(10000000))
            {
                await stream.ReadAsync(result.ContentBuffer);
            }

            return result;
        }
        public void Close()
        {
            BlazoredModal.Cancel();
        }
    }
}

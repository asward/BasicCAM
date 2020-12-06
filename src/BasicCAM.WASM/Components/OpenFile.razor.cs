using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tewr.Blazor.FileReader;

namespace BasicCAM.WASM.Components
{
    public partial class OpenFile
    {
        [CascadingParameter]
        BlazoredModalInstance BlazoredModal { get; set; }

        [Inject]
        public IFileReaderService fileReaderService { get; set; }

        public ElementReference inputTypeFileElement { get; set; }
        public async Task SubmitForm()
        {
            var files = await fileReaderService.CreateReference(inputTypeFileElement).EnumerateFilesAsync();
            var file = files.First();
            byte[] contentBuffer;
            // Read file fully into memory and act
            using (MemoryStream memoryStream = await file.CreateMemoryStreamAsync(4096))
            {
                contentBuffer = new byte[memoryStream.Length];

                // Sync calls are ok once file is in memory
                memoryStream.Read(contentBuffer);
            }


            await BlazoredModal.Close(ModalResult.Ok<byte[]>(contentBuffer));
        }

        public void Cancel()
        {
            BlazoredModal.Cancel();
        }
    }
}

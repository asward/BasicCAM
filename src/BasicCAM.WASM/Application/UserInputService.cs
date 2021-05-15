using Blazored.Modal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasicCAM.WASM.Components;
using Blazored.Modal;

namespace BasicCAM.WASM.Application { 
    public interface IUserInputService
    {
        Task<bool> VerifyAction(string title, string prompt, string yes = "Yes", string no = "No");
    }


    public class UserInputService : IUserInputService
    {
        public UserInputService(IModalService modal)
        {
            Modal = modal;
        }

        public IModalService Modal { get; }

        public async Task<bool> VerifyAction(string title, string prompt, string yes = "Yes", string no = "No")
        {
            var parameters = new ModalParameters();
            parameters.Add("Title", title);
            parameters.Add("Prompt", prompt);
            parameters.Add("YesString", yes);
            parameters.Add("NoString", no);


            var openFileModal = Modal.Show<YesNoModal>(title,parameters);
            var result = await openFileModal.Result;

            return !result.Cancelled;
        }
    }

}

using Microsoft.AspNetCore.Components;
using ProInUG.BlazorUI.Models;

namespace ProInUG.BlazorUI.Components
{
    public partial class LoginFormComponent
    {
        [Parameter]
        public EventCallback<Credentials> OnSubmitCallback { get; set; }

        [Parameter]
        public bool IsSubmitButtonDisabled { get; set; }

        [Parameter]
        public string LoginResultMessage { get; set; } = "";

        private Credentials credentials = new();

        private bool isErrorMessageHidden() => LoginResultMessage.Length == 0;
    }
}

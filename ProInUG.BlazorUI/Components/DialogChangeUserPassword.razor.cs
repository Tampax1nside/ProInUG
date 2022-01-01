using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ProInUG.BlazorUI.Components
{
    public partial class DialogChangeUserPassword
    {
        [CascadingParameter] private MudDialogInstance? MudDialog { get; set; }

        [Parameter] public ChangePasswordViewModel ChangePasswordViewModel { get; set; } = new();
        
        [Parameter] public string SubmitButtonName { get; set; } = "Change password";

        [Parameter] public string TextContent { get; set; } = "";
        
        private void Cancel() => MudDialog?.Cancel();

        private void Submit() => MudDialog?.Close(DialogResult.Ok(ChangePasswordViewModel));
    }
}
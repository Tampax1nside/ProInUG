using Microsoft.AspNetCore.Components;
using MudBlazor;
using ProInUG.BlazorUI.Dto;

namespace ProInUG.BlazorUI.Components
{
    public partial class DialogCreateUser
    {
        [CascadingParameter] private MudDialogInstance? MudDialog { get; set; }

        [Parameter] public CreateAccountDto User { get; set; } = new();
        
        [Parameter] public string SubmitButtonName { get; set; } = "Create";

        [Parameter] public string TextContent { get; set; } = "";
        
        private void Cancel() => MudDialog?.Cancel();

        private void Submit() => MudDialog?.Close(DialogResult.Ok(User));
    }
}
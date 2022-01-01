using Microsoft.AspNetCore.Components;
using MudBlazor;
using ProInUG.BlazorUI.Dto;
using ProInUG.BlazorUI.Models;

namespace ProInUG.BlazorUI.Components
{
    public partial class DialogDeleteUser
    {
        [CascadingParameter] private MudDialogInstance? MudDialog { get; set; }

        [Parameter]
        public AccountDto User { get; set; } = new();

        private void Cancel()
        {
            MudDialog?.Cancel();
        }

        private void DeleteUser()
        {
            MudDialog?.Close(DialogResult.Ok(User.Id));
        }
    }
}
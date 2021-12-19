using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ProInUG.BlazorUI.Components;
using ProInUG.BlazorUI.Models;
using ProInUG.BlazorUI.Services;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Pages
{
    public partial class LoginPage
    {
        [Inject]
        public AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        [Inject]
        public IDialogService? Dialog { get; set; }

        private Credentials credentials = new();

        private bool isSubmitButtonDisabled;
        private string loginResultMessage = "";

        private async Task LoginAsync(Credentials credentials)
        {
            if (AuthenticationStateProvider == null)
                return;

            var asp = (CwAuthenticationStateProvider)AuthenticationStateProvider;

            isSubmitButtonDisabled = true;
            var result = await asp.LoginAsync(credentials);

            if (result == 401)
            {
                loginResultMessage = "Wrong username or password.";
                ErrorMessageDialog(loginResultMessage);
                isSubmitButtonDisabled = false;
                return;
            }

            if (result != 0)
            {
                loginResultMessage = "Error ocured while login.";
                ErrorMessageDialog(loginResultMessage);
                isSubmitButtonDisabled = false;
                return;
            }

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            isSubmitButtonDisabled = false;

            if (authState.User.Identity == null || NavigationManager == null)
                return;

            NavigationManager.NavigateTo("/");
        }

        private void ErrorMessageDialog(string message)
        {
            DialogOptions options = new() { CloseButton = true };

            var parameters = new DialogParameters();
            parameters.Add("ContentText", message);
            parameters.Add("ButtonText", "Close");
            parameters.Add("Color", Color.Error);
            Dialog?.Show<DialogLoginPage>("Login Error", parameters);
        }
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ProInUG.BlazorUI.Models;
using ProInUG.BlazorUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Pages
{
    public partial class LoginPage
    {
        [Inject]
        public AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
        [Inject]
        public NavigationManager? NavigationManager { get; set; }

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
                isSubmitButtonDisabled = false;
                return;
            }

            if (result != 0)
            {
                loginResultMessage = "Error ocured while login.";
                isSubmitButtonDisabled = false;
                return;
            }

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            isSubmitButtonDisabled = false;

            if (authState.User.Identity == null || NavigationManager == null)
                return;

            NavigationManager.NavigateTo("/");
        }
    }
}

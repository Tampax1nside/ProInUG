using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ProInUG.BlazorUI.Components;
using ProInUG.BlazorUI.Models;
using ProInUG.BlazorUI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Pages
{
    public partial class Index
    {
        [Inject] public AuthenticationStateProvider? AuthenticationStateProvider { get; set; }

        // TODO: не место этому тут - просто тестил потом куда-то уйдет
        private async Task LogoutAsync()
        {
            if (AuthenticationStateProvider == null)
            {
                return;
            }

            var asp = (CwAuthenticationStateProvider)AuthenticationStateProvider;
            await asp.LogoutAsync();
        }
    }
}

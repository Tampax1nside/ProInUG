using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ProInUG.BlazorUI.Services;

namespace ProInUG.BlazorUI.Components
{
    public partial class AuthorizedUserMenu
    {
        [Inject] public AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
        
        private IEnumerable<Claim> _claims = Enumerable.Empty<Claim>();

        private ClaimsPrincipal? _user = new ClaimsPrincipal();

        protected override async Task OnInitializedAsync()
        {
            await GetClaimsPrincipalData();
        }

        private async Task GetClaimsPrincipalData()
        {
            if(AuthenticationStateProvider == null)
                return;
            
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            _user = authState.User;
            if (_user.Identity != null && _user.Identity.IsAuthenticated)
            {
                _claims = _user.Claims;
            }
        }
        
        private async Task LogoutAsync()
        {
            if (AuthenticationStateProvider == null)
            {
                return;
            }

            // TODO: убрать
            await Task.Delay(2000);
            var asp = (CwAuthenticationStateProvider)AuthenticationStateProvider;
            await asp.LogoutAsync();
        }
    }
}
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ProInUG.BlazorUI.Models;
using ProInUG.BlazorUI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Pages
{
    public partial class Index
    {
        [Inject]
        public AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
        
        [Inject]
        public IKktCloudService? KktCloudService { get; set; }

        private List<PaymentPoint>? paymentPoints;

        protected async override Task OnInitializedAsync()
        {
            await GetPaymentPointsAsync();
        }

        private async Task GetPaymentPointsAsync()
        {
            if (KktCloudService == null)
                return;

            paymentPoints = await KktCloudService.GetPaymentPointsAsync();
        }

        private async Task DeletePaymentPointAsync(Guid pointId)
        {
            if (KktCloudService == null)
                return;
            await KktCloudService.DeletePaymentPointAsync(pointId);

            await GetPaymentPointsAsync();
        }

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

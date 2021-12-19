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
    public partial class CreatePaymentPointPage
    {
        [Inject]
        public AuthenticationStateProvider? AuthenticationStateProvider { get; set; }

        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        [Inject]
        public IKktCloudService? KktCloudService { get; set; }

        private PaymentPoint paymentPoint = new();

        private bool isSubmitButtonDisabled;

        private async Task OnSubmitAsync(PaymentPoint point)
        {
            if (KktCloudService == null)
                return;

            isSubmitButtonDisabled = true;
            await KktCloudService.CreatePaymentPointAsync(point);
            isSubmitButtonDisabled = false;

            if (NavigationManager == null)
                return;

            NavigationManager.NavigateTo("/");
        }

        private void GoBack()
        {
            if (NavigationManager == null)
                return;

            NavigationManager.NavigateTo("/");
        }
    }
}

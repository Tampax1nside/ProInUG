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
        [Inject]
        public AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
        
        [Inject]
        public IKktCloudService? KktCloudService { get; set; }

        [Inject]
        public IDialogService? DialogService { get; set; }

        private List<PaymentPoint> paymentPoints = new();

        protected async override Task OnInitializedAsync()
        {
            await GetPaymentPointsAsync();
        }

        /// <summary>
        /// Создает новую точку оплаты
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private async Task CreatePaymentPointAsync()
        {
            if (KktCloudService == null)
                return;

            DialogOptions options = new DialogOptions() { MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
            var parameters = new DialogParameters 
            {
                ["SubmitButtonName"] = "Create"
            };
            var dialogForm = DialogService?.Show<DialogEditPaymentPoint>("Create Payment Point", parameters, options);

            if (dialogForm == null)
                return;

            var result = await dialogForm.Result;

            if (result.Cancelled)
                return;

            var dialogProcess = ProcessMessageDialog("processind ...");

            await Task.Delay(2000);
            dialogProcess?.Close();

            //isSubmitButtonDisabled = true;
            //await KktCloudService.CreatePaymentPointAsync(point);
            //isSubmitButtonDisabled = false;

            //if (NavigationManager == null)
            //    return;

            //NavigationManager.NavigateTo("/");
        }

        private async Task GetPaymentPointsAsync()
        {
            if (KktCloudService == null)
                return;

            var points = await KktCloudService.GetPaymentPointsAsync();
            if (points != null)
                paymentPoints = points;
        }

        private async Task DeletePaymentPointAsync(PaymentPoint point)
        {
            if (KktCloudService == null)
                return;

            DialogOptions options = new DialogOptions() { MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
            var parameters = new DialogParameters { ["paymentPoint"] = point };
            var dialog = DialogService?.Show<DialogDeletePaymentPoint>("Delete Payment Point", parameters, options);
            
            if (dialog == null)
                return;
            
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                Guid.TryParse(result.Data.ToString(), out Guid pointId);
                await KktCloudService.DeletePaymentPointAsync(pointId);
            }

            await GetPaymentPointsAsync();
        }

        private IDialogReference? ProcessMessageDialog(string message)
        {
            DialogOptions options = new() { DisableBackdropClick = true };

            var parameters = new DialogParameters();
            parameters.Add("ContentText", message);
            //parameters.Add("ButtonText", "Close");
            //parameters.Add("Color", Color.Primary);
            return DialogService?.Show<DialogProcess>("In progress", parameters, options);
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

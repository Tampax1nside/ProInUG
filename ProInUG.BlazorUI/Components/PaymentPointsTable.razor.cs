using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ProInUG.BlazorUI.Models;
using ProInUG.BlazorUI.Services;

namespace ProInUG.BlazorUI.Components
{
    public partial class PaymentPointsTable
    {
        #region DI
        [Inject] public AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
        [Inject] public IKktCloudService? KktCloudService { get; set; }
        [Inject] public IDialogService? DialogService { get; set; }
        
        #endregion

        #region Parameters
        [Parameter] public List<PaymentPoint> PaymentPoints { get; set; } = new();
       
        #endregion

        private string _searchString = "";
        private bool _loading;
        private bool _itemsChanged;

        protected override async Task OnInitializedAsync()
        {
            await GetPaymentPointsAsync();
        }

        /// <summary>
        /// Удалить точку оплаты
        /// </summary>
        /// <param name="point"></param>
        private async Task DeletePaymentPointAsync(PaymentPoint point)
        {
            if (KktCloudService == null)
                return;

            var dialogWindow = ShowDeleteDialog(point);
            
            if (dialogWindow == null)
                return;
            
            var result = await dialogWindow.Result;

            if (!result.Cancelled && Guid.TryParse(result.Data.ToString(), out var pointId))
            {
                await KktCloudService.DeletePaymentPointAsync(pointId);
            }

            _itemsChanged = true;
            await GetPaymentPointsAsync();
        }
        
        /// <summary>
        /// Создать новую точку оплаты
        /// </summary>
        /// <returns></returns>
        private async Task CreatePaymentPointAsync()
        {
            if (KktCloudService == null)
                return;

            var createDialogForm = ShowCreateDialog();
            if (createDialogForm == null)
                return;
            var result = await createDialogForm.Result;
            if (result.Cancelled)
                return;

            var dialogProcess = ShowProcessDialog("processing ...");

            // TODO: удалить
            await Task.Delay(2000);

            var creatingResult = await KktCloudService.CreatePaymentPointAsync((PaymentPoint) result.Data);

            dialogProcess?.Close();

            IDialogReference? dialogMessage;

            if (creatingResult == null)
            {
                dialogMessage = ShowErrorDialog("Ошибка","Невозможно создать точку оплаты");
                if (dialogMessage == null)
                    return;
                await dialogMessage.Result;
                return;
            }

            dialogMessage = ShowSuccessDialog("PP created successfully.");
            if (dialogMessage == null)
                return;
            await dialogMessage.Result;

            _itemsChanged = true;
            await GetPaymentPointsAsync();
        }

        private async Task EditPaymentPointAsync(PaymentPoint point)
        {
            if (KktCloudService == null)
                return;
            var editDialog = ShowEditDialog(point);
            if(editDialog == null)
                return;
            var result = await editDialog.Result;
            if (result.Cancelled)
                return;
           
            var dialogProcess = ShowProcessDialog("processing ...");

            // TODO: удалить
            await Task.Delay(2000);
            var editingResponseStatusCode = await KktCloudService.EditPaymentPointAsync(point);
            
            dialogProcess?.Close();
            
            IDialogReference? dialogMessage;
            
            if (editingResponseStatusCode != 200)
            {
                dialogMessage = ShowErrorDialog("Ошибка",$"Невозможно отредактировать точку оплаты." +
                                                         $"Код ошибки: {editingResponseStatusCode}");
                if (dialogMessage == null)
                    return;
                await dialogMessage.Result;
                return;
            }

            dialogMessage = ShowSuccessDialog("PP edited successfully.");
            if (dialogMessage == null)
                return;
            await dialogMessage.Result;

            _itemsChanged = true;
            await GetPaymentPointsAsync();
        }

        private async Task GetPaymentPointsAsync()
        {
            if (KktCloudService == null)
                return;
            
            _loading = true;
            if (_itemsChanged)
            {
                StateHasChanged();
                _itemsChanged = false;
            }

            await Task.Delay(1200); // TODO: убрать
            
            var (error, requestId, points) = await KktCloudService.GetPaymentPointsAsync();
            _loading = false;
            
            // Success
            if (error == 0 && points != null)
                PaymentPoints = points;
            
            // Finished with error
            if (error != 0)
            {
                var dialogMessage = ShowErrorDialog("Ошибка",$"Невозможно получить точки оплаты <br />" +
                                                    $"Код ошибки: {error.ToString()} <br />" +
                                                    $"Request Id: {requestId}");
                if (dialogMessage == null)
                    return;
                await dialogMessage.Result;
            }
        }
        
        private IDialogReference? ShowEditDialog(PaymentPoint point)
        {
            DialogOptions options = new() { MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
            var parameters = new DialogParameters { ["SubmitButtonName"] = "Редактировать", ["PaymentPoint"] = point };
            return DialogService?.Show<DialogEditPaymentPoint>("Редактирование точки оплаты", parameters, options);
        }
        
        private IDialogReference? ShowDeleteDialog(PaymentPoint point)
        {
            DialogOptions options = new DialogOptions() { MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
            var parameters = new DialogParameters { ["paymentPoint"] = point };
            return DialogService?.Show<DialogDeletePaymentPoint>("Delete Payment Point", parameters, options);
        }
        
        private IDialogReference? ShowCreateDialog()
        {
            DialogOptions options = new() { MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
            var parameters = new DialogParameters { ["SubmitButtonName"] = "Создать" };
            return DialogService?.Show<DialogEditPaymentPoint>("Создание точки оплаты", parameters, options);
        }
        
        private IDialogReference? ShowProcessDialog(string message)
        {
            DialogOptions options = new() { DisableBackdropClick = true };
            var parameters = new DialogParameters {{"ContentText", message}};
            return DialogService?.Show<DialogProcess>("In progress", parameters, options);
        }
        
        private IDialogReference? ShowErrorDialog(string title, string message)
        {
            DialogOptions options = new() { CloseButton = true };
            var parameters = new DialogParameters
            {
                {"ContentText", message},
                {"ButtonText", "Закрыть"},
                {"Color", Color.Error}
            };
            return DialogService?.Show<DialogLoginPage>(title, parameters, options);
        }

        private IDialogReference? ShowSuccessDialog(string message)
        {
            DialogOptions options = new() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true};
            var parameters = new DialogParameters
            {
                {"ContentText", message},
                {"ButtonText", "Close"},
                {"Color", Color.Default}
            };
            return DialogService?.Show<DialogLoginPage>("Creating PP Success", parameters, options);
        }

        private bool FilterFunc(PaymentPoint point)
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;
            return point.DeviceUri.Contains(_searchString, StringComparison.OrdinalIgnoreCase) || 
                   point.OperatorName.Contains(_searchString, StringComparison.OrdinalIgnoreCase);
        }
    }
}
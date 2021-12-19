using Microsoft.AspNetCore.Components;
using MudBlazor;
using ProInUG.BlazorUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Components
{
    public partial class DialogDeletePaymentPoint
    {
        [CascadingParameter]
        MudDialogInstance? MudDialog { get; set; }

        [Parameter]
        public PaymentPoint paymentPoint { get; set; } = new();

        private void Cancel()
        {
            if (MudDialog != null)
                MudDialog.Cancel();
        }

        private void DeletePaymentPoint()
        {
            //In a real world scenario this bool would probably be a payment point to delete the item from api/database
            //Snackbar.Add("Server Deleted", Severity.Success);
            if (MudDialog != null)
                MudDialog.Close(DialogResult.Ok(paymentPoint.Id));
        }
    }
}

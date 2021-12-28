using Microsoft.AspNetCore.Components;
using MudBlazor;
using ProInUG.BlazorUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Components
{
    public partial class DialogEditPaymentPoint
    {
        [CascadingParameter] private MudDialogInstance? MudDialog { get; set; }

        [Parameter] public PaymentPoint PaymentPoint { get; set; } = new();

        [Parameter] public string SubmitButtonName { get; set; } = "Create";

        [Parameter] public string TextContent { get; set; } = "";
        
        private void Cancel()
        {
            if (MudDialog != null)
                MudDialog.Cancel();
        }

        private void CreatePaymentPoint()
        {
            if (MudDialog != null)
                MudDialog.Close(DialogResult.Ok(PaymentPoint));
        }
    }
}

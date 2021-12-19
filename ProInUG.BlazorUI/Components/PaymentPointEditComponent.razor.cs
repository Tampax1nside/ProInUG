using Microsoft.AspNetCore.Components;
using ProInUG.BlazorUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Components
{
    public partial class PaymentPointEditComponent
    {
        [Parameter]
        public EventCallback<PaymentPoint> OnSubmitCallback { get; set; }

        [Parameter]
        public EventCallback OnCancelCallback { get; set; }

        [Parameter]
        public PaymentPoint paymentPoint { get; set; } = new();

        [Parameter]
        public string FormName { get; set; } = "";

        [Parameter]
        public string SubmitButtonName { get; set; } = "Create";

        [Parameter]
        public bool IsSubmitButtonDisable { get; set; }
    }
}

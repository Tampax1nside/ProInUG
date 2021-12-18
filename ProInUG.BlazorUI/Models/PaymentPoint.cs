using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Models
{
    public class PaymentPoint
    {
        public Guid Id { get; set; }

        public bool IsEnable { get; set; }

        public Guid AccountId { get; set; }

        [Required]
        public string DeviceUri { get; set; } = "";

        [StringLength(128, MinimumLength = 10)]
        public string PaymentAddress { get; set; } = "";

        [StringLength(64, MinimumLength = 4)]
        public string OperatorName { get; set; } = "";

        [StringLength(64, MinimumLength = 4)]
        public string NomenclatureName { get; set; } = "";

        public TaxationType TaxationType { get; set; }

        public TaxRate TaxRate { get; set; }
    }
}

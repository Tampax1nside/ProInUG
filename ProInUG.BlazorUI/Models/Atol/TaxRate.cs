namespace ProInUG.BlazorUI.Models
{
    public enum TaxRate
    {
        /// <summary>
        /// налогом не облагается
        /// </summary>
        None,
        /// <summary>
        /// НДС 0%
        /// </summary>
        Vat0,
        /// <summary>
        /// НДС 10%
        /// </summary>
        Vat10,
        /// <summary>
        /// НДС 18%
        /// </summary>
        Vat18,
        /// <summary>
        /// НДС 10/110
        /// </summary>
        Vat110,
        /// <summary>
        /// НДС 18/118
        /// </summary>
        Vat118,
        /// <summary>
        /// НДС 20%
        /// </summary>
        Vat20,
        /// <summary>
        /// НДС 20/120	
        /// </summary>
        Vat120
    }
}

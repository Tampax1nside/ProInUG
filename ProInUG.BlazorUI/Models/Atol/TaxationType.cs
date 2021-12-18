namespace ProInUG.BlazorUI.Models
{
    public enum TaxationType
    {
        /// <summary>
        /// общая
        /// </summary>
        Osn,
        /// <summary>
        /// упрощенная (Доход)
        /// </summary>
        UsnIncome,
        /// <summary>
        /// упрощенная (Доход минус Расход)
        /// </summary>
        UsnIncomeOutcome,
        /// <summary>
        /// ЕНВД
        /// </summary>
        Envd,
        /// <summary>
        /// единый сельскохозяйственный налог
        /// </summary>
        Esn,
        /// <summary>
        /// патентная
        /// </summary>
        Patent
    }
}

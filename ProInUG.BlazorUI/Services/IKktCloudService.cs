using ProInUG.BlazorUI.Dto;
using ProInUG.BlazorUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Services
{
    /// <summary>
    /// Сервис по работе с API KKT
    /// </summary>
    public interface IKktCloudService
    {
        /// <summary>
        /// Получить список всех точек оплаты
        /// </summary>
        /// <returns></returns>
        Task<List<PaymentPoint>?> GetPaymentPointsAsync();

        /// <summary>
        /// Создать новую точку оплаты
        /// </summary>
        /// <param name="paymentPoint"></param>
        /// <returns></returns>
        Task<PaymentPoint?> CreatePaymentPointAsync(PaymentPoint paymentPoint);

        /// <summary>
        /// Удалить точку оплаты
        /// </summary>
        /// <param name="pointId"></param>
        /// <returns></returns>
        Task DeletePaymentPointAsync(Guid pointId);

        /// <summary>
        /// Редактировать точку доступа
        /// </summary>
        /// <param name="paymentPoint"></param>
        /// <returns></returns>
        Task<int> EditPaymentPointAsync(PaymentPoint paymentPoint);
    }
}

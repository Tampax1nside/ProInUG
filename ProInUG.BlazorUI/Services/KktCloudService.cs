using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProInUG.BlazorUI.Extentions;
using ProInUG.BlazorUI.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Services
{
    /// <summary>
    /// Сервис по работе с API KKT
    /// </summary>
    public class KktCloudService : IKktCloudService
    {
        private const string PAYMENT_POINTS_ENDPOINT = "PaymentPoints";
        private readonly HttpClient _client;
        private readonly ILogger<KktCloudService> _logger;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="client"></param>
        /// <param name="authenticationStateProvider"></param>
        /// <param name="logger"></param>
        public KktCloudService(
            HttpClient client, 
            AuthenticationStateProvider authenticationStateProvider,
            ILogger<KktCloudService> logger)
        {
            _client = client;
            _authenticationStateProvider = authenticationStateProvider;
            _logger = logger;
        }

        /// <summary>
        /// Создать новую точку оплаты
        /// </summary>
        /// <param name="paymentPoint"></param>
        /// <returns></returns>
        public async Task<PaymentPoint?> CreatePaymentPointAsync(PaymentPoint paymentPoint)
        {
            var api = "v0.0";
            var uri = $"{api}/{PAYMENT_POINTS_ENDPOINT}";

            var jwt = GetJwt();
            if (string.IsNullOrEmpty(jwt))
                return null;

            try
            {
                var response = await _client.PostAsJson(paymentPoint, uri, jwt);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return await response.Content.ReadAs<PaymentPoint>();

                    case HttpStatusCode.Unauthorized:
                        await LogoutAsync();
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new payment point.");
            }
            return null;
        }

        /// <summary>
        /// Получить список всех точек оплаты
        /// </summary>
        /// <returns></returns>
        public async Task<(int Error, string RequestId, List<PaymentPoint>? Points)> GetPaymentPointsAsync()
        {
            var api = "v0.0";
            var uri = $"{api}/{PAYMENT_POINTS_ENDPOINT}";

            var requestId = _client.GenerateRequestId();
            
            var jwt = GetJwt();
            if (string.IsNullOrEmpty(jwt))
                return ((int) HttpStatusCode.Unauthorized, requestId, null);

            try
            {
                _logger.LogDebug($"RequestId: [{requestId}]. Getting payment points list");
                var result = await _client.GetAsJson(uri, jwt, requestId);
                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var response = await result.Content.ReadAs<List<PaymentPoint>>();
                        return (0, requestId, response);

                    case HttpStatusCode.Unauthorized:
                        await LogoutAsync();
                        return ((int) HttpStatusCode.Unauthorized, requestId, null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"RequestId: [{requestId}]. Error occured getting payment points list.");
            }
            return (1000, requestId, null);
        }

        /// <summary>
        /// Удалить точку оплаты
        /// </summary>
        /// <param name="pointId"></param>
        /// <returns></returns>
        public async Task DeletePaymentPointAsync(Guid pointId)
        {
            var api = "v0.0";
            var uri = $"{api}/{PAYMENT_POINTS_ENDPOINT}/{pointId}";
            var jwt = GetJwt();
            if (string.IsNullOrEmpty(jwt))
                return;
            try
            {
                var response = await _client.DeleteAsJson(uri, null, jwt); // TODO: коммент в экстеншнах по этому поводу
                
                if (!response.IsSuccessStatusCode)
                {
                    await LogoutAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting payment point.");
            }
        }

        /// <summary>
        /// Редактировать точку доступа
        /// </summary>
        /// <param name="paymentPoint"></param>
        /// <returns></returns>
        public async Task<int> EditPaymentPointAsync(PaymentPoint paymentPoint)
        {
            var api = "v0.0";
            var uri = $"{api}/{PAYMENT_POINTS_ENDPOINT}/{paymentPoint.Id}";
            var jwt = GetJwt();
            if (string.IsNullOrEmpty(jwt))
                return (int) HttpStatusCode.Unauthorized;
            try
            {
                var response = await _client.PatchAsJson(uri, paymentPoint, jwt);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    await LogoutAsync();
                return (int) response.StatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing payment point.");
            }

            return 1000;
        }

        private string? GetJwt()
        {
            var asp = (CwAuthenticationStateProvider)_authenticationStateProvider;
            return asp.TokenDto?.Jwt;
        }

        private async Task LogoutAsync()
        {
            var asp = (CwAuthenticationStateProvider)_authenticationStateProvider;
            if (asp == null)
                return;
            await asp.LogoutAsync();
        }
    }
}

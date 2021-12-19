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
        public async Task<List<PaymentPoint>?> GetPaymentPointsAsync()
        {
            var api = "v0.0";
            var uri = $"{api}/{PAYMENT_POINTS_ENDPOINT}";

            var jwt = GetJwt();
            if (string.IsNullOrEmpty(jwt))
                return null;

            try
            {
                var result = await _client.GetAsJson(uri, jwt);
                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return await result.Content.ReadAs<List<PaymentPoint>>();

                    case HttpStatusCode.Unauthorized:
                        await LogoutAsync();
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ocured getting payment points list.");
            }
            return null;
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

        private string? GetJwt()
        {
            var asp = (CwAuthenticationStateProvider)_authenticationStateProvider;
            if (asp.TokenDto == null)
                return null;
            return asp.TokenDto.Jwt;
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

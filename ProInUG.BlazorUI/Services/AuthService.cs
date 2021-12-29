using Microsoft.Extensions.Logging;
using ProInUG.BlazorUI.Dto;
using ProInUG.BlazorUI.Extentions;
using ProInUG.BlazorUI.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Services
{
    public class AuthService : IAuthService
    {
        private const string API = "api";
        private const string LOGIN_ENDPOINT = "Token/signin";
        private const string REFRESH_ENDPOINT = "Token/refreshId=";

        private readonly HttpClient _client;
        private readonly ILogger<AuthService> _logger;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public AuthService(HttpClient client, ILogger<AuthService> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Вход пользователя
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public async Task<(int Error, TokenDto? Token)> LoginAsync(Credentials credentials)
        {
            var uri = $"{API}/{LOGIN_ENDPOINT}";

            try
            {
                var response = await _client.SendAsJson(HttpMethod.Post, new 
                { 
                    name = credentials.Username, 
                    password = credentials.Password 
                },
                uri);

                if (response.StatusCode != HttpStatusCode.OK) return ((int) response.StatusCode, null);
                var tokenDto = await response.Content.ReadAs<TokenDto>();
                return (0, tokenDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while login");
                return (1000, null);
            }
        }

        /// <summary>
        /// Обновить токен
        /// </summary>
        /// <param name="token">Refreshing token</param>
        /// <returns></returns>
        public async Task<(int Error, TokenDto? Token)> RefreshToken(TokenDto token)
        {
            var uri = $"{API}/{REFRESH_ENDPOINT}{token.RefreshTokenId}";

            try
            {
                var response = await _client.SendAsJson(HttpMethod.Post, null, uri, token.Jwt);

                if (response.StatusCode != HttpStatusCode.OK) return ((int) response.StatusCode, null);
                var tokenDto = await response.Content.ReadAs<TokenDto>();
                return (0, tokenDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while token refresh.");
                return (1000, null);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using ProInUG.BlazorUI.Dto;
using ProInUG.BlazorUI.Extentions;

namespace ProInUG.BlazorUI.Services
{
    /// <summary>
    /// Сервис работы с аккаунтами пользователей
    /// </summary>
    public class UsersService : IUsersService
    {
        private const string API = "api";
        private const string ACCOUNT_ENDPOINT = "/Account";
        
        private readonly HttpClient _client;
        private readonly ILogger<UsersService> _logger;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="client"></param>
        /// <param name="authenticationStateProvider"></param>
        /// <param name="logger"></param>
        public UsersService(
            HttpClient client,
            AuthenticationStateProvider authenticationStateProvider,
            ILogger<UsersService> logger)
        {
            _client = client;
            _authenticationStateProvider = authenticationStateProvider;
            _logger = logger;
        }
        
        /// <summary>
        /// Получить текущий аккаунт
        /// </summary>
        /// <returns></returns>
        public async Task<(int Error, AccountDto? Account)> GetCurrentAccount()
        {
            var uri = $"{API}/{ACCOUNT_ENDPOINT}";
            try
            {
                _logger.LogDebug($"Запрос API получение текущего аккаунта по Uri: {uri}");
                var jwt = GetJwt();
                if (string.IsNullOrEmpty(jwt))
                    return ((int) HttpStatusCode.Unauthorized, null);
                var response = await _client.GetAsJson(uri, jwt);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAs<AccountDto>();
                    return (0, data);
                }

                if (response.StatusCode == HttpStatusCode.Forbidden ||
                    response.StatusCode == HttpStatusCode.Unauthorized ||
                    response.StatusCode == HttpStatusCode.InternalServerError)
                    return ((int) response.StatusCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка получения текущего аккаунта.");
            }

            return (1000, null);
        }

        public Task<(int Error, AccountDto Account)> CreateAccount(AccountDto account)
        {
            throw new System.NotImplementedException();
        }

        public Task<(int Error, AccountDto Account)> UpdateAccount(AccountDto account)
        {
            throw new System.NotImplementedException();
        }

        public Task<(int Error, List<AccountDto> Account)> GetAccounts()
        {
            throw new System.NotImplementedException();
        }

        public Task<int> DeleteAccount(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> ChangeUserPassword(string userId, string password)
        {
            throw new System.NotImplementedException();
        }
        
        // TODO: в вдух местах один код - исправить
        private string? GetJwt()
        {
            var asp = (CwAuthenticationStateProvider)_authenticationStateProvider;
            return asp.TokenDto?.Jwt;
        }
    }
}
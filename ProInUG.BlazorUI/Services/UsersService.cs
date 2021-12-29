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
        private const string ACCOUNT_ENDPOINT = "Account";
        private const string ALL_ACCOUNTS_ENDPOINT = "Account/Current";
        private const string PASSWORD_ENDPOINT = "Account/password";
        
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
                
                var response = await _client.SendAsJson(HttpMethod.Get, null, uri, jwt);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAs<AccountDto>();
                    return (0, data);
                }

                if (response.StatusCode is HttpStatusCode.Forbidden or 
                    HttpStatusCode.Unauthorized or 
                    HttpStatusCode.InternalServerError)
                    return ((int) response.StatusCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка получения текущего аккаунта.");
            }

            return (1000, null);
        }

        /// <summary>
        /// Создать новый аккаунт
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<(int Error, AccountDto? Account)> CreateAccount(CreateAccountDto account)
        {
            var uri = $"{API}/{ACCOUNT_ENDPOINT}";
            try
            {
                _logger.LogDebug($"Запрос API на создание нового аккаунта по Uri: {uri}");
                var jwt = GetJwt();
                if (string.IsNullOrEmpty(jwt))
                    return ((int) HttpStatusCode.Unauthorized, null);
                
                var response = await _client.SendAsJson(HttpMethod.Put, account, uri, jwt);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAs<AccountDto>();
                    return (0, data);
                }

                if (response.StatusCode is HttpStatusCode.Forbidden or 
                    HttpStatusCode.Unauthorized or 
                    HttpStatusCode.InternalServerError or 
                    HttpStatusCode.Conflict)
                    return ((int) response.StatusCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка создания нового аккаунта.");
            }
            
            return (1000, null);
        }

        /// <summary>
        /// Изменить аккаунт
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<(int Error, AccountDto? Account)> UpdateAccount(AccountDto account)
        {
            var uri = $"{API}/{ACCOUNT_ENDPOINT}";
            try
            {
                _logger.LogDebug($"Запрос API на редактирования аккаунта по Uri: {uri}");
                var jwt = GetJwt();
                if (string.IsNullOrEmpty(jwt))
                    return ((int) HttpStatusCode.Unauthorized, null);
                
                var response = await _client.SendAsJson(HttpMethod.Patch, account, uri, jwt);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAs<AccountDto>();
                    return (0, data);
                }

                if (response.StatusCode is HttpStatusCode.Forbidden or 
                    HttpStatusCode.Unauthorized or 
                    HttpStatusCode.InternalServerError or 
                    HttpStatusCode.Conflict)
                    return ((int) response.StatusCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка редактирования аккаунта.");
            }
            
            return (1000, null);
        }
        
        /// <summary>
        /// Получить все аккаунты
        /// </summary>
        /// <returns></returns>
        public async Task<(int Error, List<AccountDto>? Account)> GetAccounts()
        {
            var uri = $"{API}/{ALL_ACCOUNTS_ENDPOINT}";
            try
            {
                _logger.LogDebug($"Запрос API получение списка всех аккаунтов по Uri: {uri}");
                var jwt = GetJwt();
                if (string.IsNullOrEmpty(jwt))
                    return ((int) HttpStatusCode.Unauthorized, null);
                
                var response = await _client.SendAsJson(HttpMethod.Get, null, uri, jwt);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAs<List<AccountDto>>();
                    return (0, data);
                }

                if (response.StatusCode is HttpStatusCode.Forbidden or 
                    HttpStatusCode.Unauthorized or 
                    HttpStatusCode.InternalServerError or 
                    HttpStatusCode.NoContent)
                    return ((int) response.StatusCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка получения списка аккаунтов.");
            }
            
            return (1000, null);
        }
        
        /// <summary>
        /// Удалить аккаунт
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> DeleteAccount(string userId)
        {
            var uri = $"{API}/{ACCOUNT_ENDPOINT}/id={userId}";
            try
            {
                _logger.LogDebug($"Запрос API удаление аккаунта по Uri: {uri}");
                var jwt = GetJwt();
                if (string.IsNullOrEmpty(jwt))
                    return (int) HttpStatusCode.Unauthorized;
                
                var response = await _client.SendAsJson(HttpMethod.Delete, null, uri, jwt);
                if (response.IsSuccessStatusCode)
                {
                    return 0;
                }

                if (response.StatusCode is HttpStatusCode.Forbidden or 
                    HttpStatusCode.Unauthorized or 
                    HttpStatusCode.InternalServerError or 
                    HttpStatusCode.NotFound)
                    return (int) response.StatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка удаления аккаунта.");
            }
            
            return 1000;
        }

        /// <summary>
        /// Изменить пароль для аккаунта
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<int> ChangeUserPassword(string userId, string password)
        {
            var uri = $"{API}/{PASSWORD_ENDPOINT}/accountid={userId}";
            try
            {
                _logger.LogDebug($"Запрос API изменение пароля аккаунта по Uri: {uri}");
                var jwt = GetJwt();
                if (string.IsNullOrEmpty(jwt))
                    return (int) HttpStatusCode.Unauthorized;
                
                var response = await _client.SendAsJson(HttpMethod.Patch, password, uri, jwt);
                if (response.IsSuccessStatusCode)
                {
                    return 0;
                }

                if (response.StatusCode is HttpStatusCode.Forbidden or 
                    HttpStatusCode.Unauthorized or 
                    HttpStatusCode.InternalServerError or 
                    HttpStatusCode.NotFound)
                    return (int) response.StatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка изменения пароля для аккаунта.");
            }
            
            return 1000;
        }
        
        // TODO: в вдух местах один код - исправить
        private string? GetJwt()
        {
            var asp = (CwAuthenticationStateProvider)_authenticationStateProvider;
            return asp.TokenDto?.Jwt;
        }
    }
}
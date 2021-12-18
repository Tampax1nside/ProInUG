using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using ProInUG.BlazorUI.Dto;
using ProInUG.BlazorUI.Models;
using System;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Services
{
    /// <summary>
    /// Провайдер состояния аутентификации
    /// </summary>
    public class CwAuthenticationStateProvider : AuthenticationStateProvider
    {
        private const string USER_SESSION_OBJECT_KEY = "user_session_obj";
        private const int TOKEN_VALID_MINUTES_REMAINS = 20; // TODO: в настройки приложения

        public TokenDto? TokenDto { get; set; }

        private readonly IAuthService _authService;
        private readonly ProtectedLocalStorage _protectedLocalStorage;
        private readonly ISystemClock _systemClock;
        private readonly ILogger<CwAuthenticationStateProvider> _logger;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="protectedLocalStorage"></param>
        public CwAuthenticationStateProvider(
            IAuthService authService,
            ProtectedLocalStorage protectedLocalStorage,
            ISystemClock systemClock,
            ILogger<CwAuthenticationStateProvider> logger)
        {
            _authService = authService;
            _protectedLocalStorage = protectedLocalStorage;
            _systemClock = systemClock;
            _logger = logger;
        }

        /// <summary>
        /// Поучить состояние аутентификации
        /// </summary>
        /// <returns></returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            TokenDto = await GetUserSessionAsync();

            if (TokenDto == null)
            {
                return GenerateEmptyAuthenticationState();
            }

            if (TokenIsValid())
            {
                return GenerateAuthenticationState(TokenDto);
            }

            // токен еще не истек
            if (TokenDto.Expires > _systemClock.UtcNow) 
            {
                var result = await _authService.RefreshToken(TokenDto);
                if (result.Error == 0 && result.Token != null)
                {
                    await _protectedLocalStorage.SetAsync(USER_SESSION_OBJECT_KEY, JsonSerializer.Serialize(result.Token));
                    TokenDto = result.Token;
                    return GenerateAuthenticationState(TokenDto);
                }
            }

            // токен истек
            await LogoutAsync();
            return GenerateEmptyAuthenticationState();
        }

        /// <summary>
        /// Вход пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<int> LoginAsync(Credentials credentials)
        {
            var result = await _authService.LoginAsync(credentials);
            if (result.Error != 0)
            {
                return result.Error;
            }
            if (result.Token != null)
            {
                try
                {
                    await _protectedLocalStorage.SetAsync(USER_SESSION_OBJECT_KEY, JsonSerializer.Serialize(result.Token));
                    NotifyAuthenticationStateChanged(Task.FromResult(GenerateAuthenticationState(result.Token)));
                    return 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error ocured writing to browser protected local storage.");
                }
            }
            return 500;
        }

        /// <summary>
        /// Выход пользователя
        /// </summary>
        /// <returns></returns>
        public async Task LogoutAsync()
        {
            TokenDto = null;
            try
            {
                await _protectedLocalStorage.DeleteAsync(USER_SESSION_OBJECT_KEY);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ocured while deleting data in browser protected local storage.");
            }
            NotifyAuthenticationStateChanged(Task.FromResult(GenerateEmptyAuthenticationState()));
        }

        /// <summary>
        /// Получить пользователя
        /// </summary>
        /// <returns></returns>
        private async Task<TokenDto?> GetUserSessionAsync()
        {
            if (TokenDto != null)
            {
                return TokenDto;
            }

            try
            {
                var tokenDto = await _protectedLocalStorage.GetAsync<string>(USER_SESSION_OBJECT_KEY);
                if (string.IsNullOrEmpty(tokenDto.Value))
                {
                    return null;
                }

                return JsonSerializer.Deserialize<TokenDto>(tokenDto.Value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user session from protected browser storage.");
                return null;
            }
        }

        /// <summary>
        /// Проверяет валидность токена по времени
        /// </summary>
        /// <returns></returns>
        private bool TokenIsValid()
        {
            if (TokenDto == null)
            {
                return false;
            }

            var validTimeRemains = TokenDto.Expires - _systemClock.UtcNow;
            _logger.LogDebug($"User token valid time remains: {validTimeRemains}");

            return validTimeRemains > TimeSpan.Zero &&
                validTimeRemains > TimeSpan.FromMinutes(TOKEN_VALID_MINUTES_REMAINS);
        }

        /// <summary>
        /// Новое состояние аутентификации
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private AuthenticationState GenerateAuthenticationState(TokenDto user)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Account.Name),
                new Claim(ClaimTypes.Role, user.Account.Role.ToString()),
            }, "auth");

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            return new AuthenticationState(claimsPrincipal);
        }

        /// <summary>
        /// Пустое состояние аутентификации
        /// </summary>
        /// <returns></returns>
        private AuthenticationState GenerateEmptyAuthenticationState() =>
            new AuthenticationState(new ClaimsPrincipal());
    }
}

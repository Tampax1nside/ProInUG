using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using ProInUG.BlazorUI.Dto;
using ProInUG.BlazorUI.Models;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ProInUG.BlazorUI.Services
{
    /// <summary>
    /// Провайдер состояния аутентификации
    /// </summary>
    public class CwAuthenticationStateProvider : AuthenticationStateProvider
    {
        private const string UserSessionObjectKey = "user_session_obj";

        public TokenDto? TokenDto { get; private set; }

        private readonly IAuthService _authService;
        private readonly ProtectedLocalStorage _protectedLocalStorage;
        private readonly ISystemClock _systemClock;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CwAuthenticationStateProvider> _logger;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="authService"></param>
        /// <param name="protectedLocalStorage"></param>
        /// <param name="systemClock"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public CwAuthenticationStateProvider(
            IAuthService authService,
            ProtectedLocalStorage protectedLocalStorage,
            ISystemClock systemClock,
            IConfiguration configuration,
            ILogger<CwAuthenticationStateProvider> logger)
        {
            _authService = authService;
            _protectedLocalStorage = protectedLocalStorage;
            _systemClock = systemClock;
            _configuration = configuration;
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
                    await _protectedLocalStorage.SetAsync(UserSessionObjectKey, JsonSerializer.Serialize(result.Token));
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
            var (error, token) = await _authService.LoginAsync(credentials);
            if (error != 0) return error;
            if (token == null) return 500;
            try
            {
                await _protectedLocalStorage.SetAsync(UserSessionObjectKey, JsonSerializer.Serialize(token));
                NotifyAuthenticationStateChanged(Task.FromResult(GenerateAuthenticationState(token)));
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred writing to browser protected local storage.");
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
                await _protectedLocalStorage.DeleteAsync(UserSessionObjectKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting data in browser protected local storage.");
            }
            NotifyAuthenticationStateChanged(Task.FromResult(GenerateEmptyAuthenticationState()));
        }

        /// <summary>
        /// Получить пользователя
        /// </summary>
        /// <returns></returns>
        private async Task<TokenDto?> GetUserSessionAsync()
        {
            if (TokenDto != null) return TokenDto;
            try
            {
                var tokenDto = await _protectedLocalStorage.GetAsync<string>(UserSessionObjectKey);
                return string.IsNullOrEmpty(tokenDto.Value) ? null : 
                    JsonSerializer.Deserialize<TokenDto>(
                        tokenDto.Value, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );
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
            if (TokenDto == null) return false;
            
            var validTimeRemains = TokenDto.Expires - _systemClock.UtcNow;
            _logger.LogDebug($"User token valid time remains: {validTimeRemains}");
            
            if(int.TryParse(_configuration["TokenValidMinutesRemains"], out var timeRemains))
                return validTimeRemains > TimeSpan.Zero &&
                       validTimeRemains > TimeSpan.FromMinutes(timeRemains);
            
            return false;
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
        private static AuthenticationState GenerateEmptyAuthenticationState() =>
            new AuthenticationState(new ClaimsPrincipal());
    }
}

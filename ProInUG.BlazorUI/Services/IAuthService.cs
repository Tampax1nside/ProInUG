using ProInUG.BlazorUI.Dto;
using ProInUG.BlazorUI.Models;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Вход пользователя
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        Task<(int Error, TokenDto? Token)> LoginAsync(Credentials credentials);

        /// <summary>
        /// Обновить токен
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<(int Error, TokenDto? Token)> RefreshToken(TokenDto token);
    }
}

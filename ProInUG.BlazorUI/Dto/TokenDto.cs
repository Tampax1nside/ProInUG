using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Dto
{
    /// <summary>
    /// Токен и данные об аккаунте
    /// </summary>
    public class TokenDto
    {
        /// <summary>
        /// Аккаунт
        /// </summary>
        public AccountDto Account { get; set; } = new();

        /// <summary>
        /// JSON Web Token
        /// </summary>
        public string Jwt { get; set; } = "";

        /// <summary>
        /// Время истечения
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Id для обновления
        /// </summary>
        public Guid RefreshTokenId { get; set; }

        /// <summary>
        /// Время истечения в тиках
        /// </summary>
        public long ExpiresJsTicks { get; set; }
    }
}

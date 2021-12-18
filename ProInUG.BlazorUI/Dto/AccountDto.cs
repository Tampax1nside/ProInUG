using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Dto
{
    public class AccountDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = "";

        public Roles Role { get; set; }

        public string Description { get; set; } = "";
    }

    /// <summary>
    /// Роли пользователей
    /// </summary>
    public enum Roles
    {
        /// <summary>
        /// Не авторизован
        /// </summary>
        NoAuthorized,

        /// <summary>
        /// Базовые
        /// </summary>
        Base = 100,

        /// <summary>
        /// Базовые
        /// </summary>
        Application = 110,

        /// <summary>
        /// Поддержка
        /// </summary>
        Support = 200,

        /// <summary>
        /// Администратор
        /// </summary>
        Administrator = 300,

        /// <summary>
        /// Супер администратор
        /// </summary>
        SuperAdministrator = 400
    }
}

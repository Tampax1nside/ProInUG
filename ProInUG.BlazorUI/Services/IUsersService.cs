using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProInUG.BlazorUI.Dto;

namespace ProInUG.BlazorUI.Services
{
    public interface IUsersService
    {
        /// <summary>
        /// Получить текущий аккаунт
        /// </summary>
        /// <returns></returns>
        Task<(int Error, AccountDto? Account)> GetCurrentAccount();

        /// <summary>
        /// Создать новый аккаунт
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Task<(int Error, AccountDto? Account)> CreateAccount(CreateAccountDto account);
        
        /// <summary>
        /// Изменить аккаунт
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Task<(int Error, AccountDto? Account)> UpdateAccount(AccountDto account);
        
        /// <summary>
        /// Получить все аккаунты
        /// </summary>
        /// <returns></returns>
        Task<(int Error, List<AccountDto>? Account)> GetAccounts();
        
        /// <summary>
        /// Удалить аккаунт
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> DeleteAccount(string userId);

        /// <summary>
        /// Изменить пароль для аккаунта
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<int> ChangeUserPassword(string userId, string password);
    }
}
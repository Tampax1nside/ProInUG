using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ProInUG.BlazorUI.Dto;
using ProInUG.BlazorUI.Services;

namespace ProInUG.BlazorUI.Components
{
    public partial class UsersTable
    {
        #region DI
        [Inject] public IUsersService? UsersService { get; set; }
        [Inject] public IDialogService? DialogService { get; set; }

        #endregion
        
        private List<UserViewModel> _usersList = new();
        private bool _loading;

        protected override async Task OnInitializedAsync()
        {
            await GetUsersListAsync();
        }

        private async Task GetUsersListAsync()
        {
            if (UsersService == null) return;
            _loading = true;
            
            // TODO: убрать
            await Task.Delay(700);
            var (error, users) = await UsersService.GetAccounts();
            _loading = false;
            
            // Success
            if (error == 0 && users != null)
            {
                _usersList.Clear();
                foreach (var user in users)
                {
                    _usersList.Add(new UserViewModel{User = user});
                }
            }
            
            // Error
            if (error != 0)
            {
                var dialogMessage = ShowErrorDialog("Ошибка",$"Невозможно получить список пользователей <br />" +
                                                             $"Код ошибки: {error.ToString()} <br />");
                if (dialogMessage == null)
                    return;
                await dialogMessage.Result;
            }
        }
        
        // TODO: один и тот же код в двух местах - переделать
        /// <summary>
        /// Показать диалог с ошибкой
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private IDialogReference? ShowErrorDialog(string title, string message)
        {
            DialogOptions options = new() { CloseButton = true };
            var parameters = new DialogParameters
            {
                {"ContentText", message},
                {"ButtonText", "Закрыть"},
                {"Color", Color.Error}
            };
            return DialogService?.Show<DialogLoginPage>(title, parameters, options);
        }
    }

    public class UserViewModel
    {
        public bool ShowDetails { get; set; }
        public AccountDto User { get; set; } = new();
    }
}
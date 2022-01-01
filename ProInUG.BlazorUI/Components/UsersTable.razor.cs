using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
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

        [Inject] public ILogger<UsersTable>? Logger { get; set; }

        #endregion
        
        private List<UserViewModel> _usersList = new();
        private bool _itemsChanged;
        private bool _loading;

        protected override async Task OnInitializedAsync()
        {
            await GetUsersListAsync();
        }

        /// <summary>
        /// Получить список пользователей
        /// </summary>
        private async Task GetUsersListAsync()
        {
            if (UsersService == null) return;
            _loading = true;
            
            if (_itemsChanged)
            {
                StateHasChanged();
                _itemsChanged = false;
            }
            
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

        /// <summary>
        /// Изменить пароль пользователя
        /// </summary>
        /// <param name="user"></param>
        private async Task ChangePasswordAsync(AccountDto user)
        {
            if (UsersService == null)
                return;

            var userViewModel = new ChangePasswordViewModel {User = user}; 
            var dialogWindow = ShowChangePasswordDialog(userViewModel);
            
            if(dialogWindow == null) return;
            
            var result = await dialogWindow.Result;

            if (!result.Cancelled && result.Data != null)
            {
                try
                {
                    userViewModel = (ChangePasswordViewModel) result.Data;
                    
                    var dialogProcess = ShowProcessDialog("processing ...");
                    // TODO: удалить
                    await Task.Delay(1000);

                    dialogProcess?.Close();

                    IDialogReference? dialogMessage;
                    var error = await UsersService.ChangeUserPassword(userViewModel.User.Id.ToString(), userViewModel.Password);
                    
                    if (error != 0)
                    {
                        dialogMessage = ShowErrorDialog("Ошибка","Невозможно изменить пароль пользователя <br />" +
                                                                 $"Код ошибки {error}");
                        if (dialogMessage == null)
                            return;
                        await dialogMessage.Result;
                        return;
                    }
                    
                    dialogMessage = ShowSuccessDialog("Изменение пароля",$"Пароль пользователя {user.Name} успешно изменен.");
                    if (dialogMessage == null)
                        return;
                    await dialogMessage.Result;
                    
                }
                catch (InvalidCastException castEx)
                {
                    Logger.LogCritical(castEx,
                        "Невозможно привести  результат диалогового окна в тип ChangePasswordViewModel.");
                }
                catch (Exception ex)
                {
                    Logger.LogCritical(ex, "Ошибка изменения пароля пользователя.");
                }
                
            }
        }

        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <param name="user"></param>
        private async Task DeleteUserAsync(AccountDto user)
        {
            if (UsersService == null)
                return;

            var dialogWindow = ShowDeleteDialog(user);
            
            if (dialogWindow == null)
                return;
            
            var result = await dialogWindow.Result;

            if (!result.Cancelled && result.Data != null)
            {
                var id = result.Data.ToString();
                if (id != null)
                {
                    var dialogProcess = ShowProcessDialog("processing ...");
                    // TODO: удалить
                    await Task.Delay(1000);

                    var error = await UsersService.DeleteAccount(id.ToString());
                    
                    dialogProcess?.Close();

                    IDialogReference? dialogMessage;
                    
                    if (error != 0)
                    {
                        dialogMessage = ShowErrorDialog("Ошибка","Невозможно удалить пользователя <br />" +
                                                                 $"Код ошибки {error}");
                        if (dialogMessage == null)
                            return;
                        await dialogMessage.Result;
                        return;
                    }

                    dialogMessage = ShowSuccessDialog("Удаление пользователя",$"Пользователь {user.Name} успешно удален");
                    if (dialogMessage == null)
                        return;
                    await dialogMessage.Result;
                    
                    _itemsChanged = true;
                    await GetUsersListAsync();   
                }
            }
        }

        /// <summary>
        /// Редактировать пользователя
        /// </summary>
        /// <param name="user"></param>
        private async Task EditUserAsync(AccountDto user)
        {
            if (UsersService == null) return;

            var editDialog = ShowEditDialog(user);
            if (editDialog == null) return;

            var result = await editDialog.Result;
            if (result.Cancelled) return;
            
            var dialogProcess = ShowProcessDialog("processing ...");
            
            // TODO: удалить
            await Task.Delay(1000);

            var (error, account) = await UsersService.UpdateAccount((AccountDto) result.Data);
            
            dialogProcess?.Close();

            IDialogReference? dialogMessage;
            
            if (error != 0 || account == null)
            {
                dialogMessage = ShowErrorDialog("Ошибка","Невозможно изменить данного пользователя <br />" +
                                                         $"Код ошибки {error}");
                if (dialogMessage == null)
                    return;
                await dialogMessage.Result;
                return;
            }

            dialogMessage = ShowSuccessDialog("Редактирование пользователя","Пользователь успешно изменен");
            if (dialogMessage == null)
                return;
            await dialogMessage.Result;

            _itemsChanged = true;
            await GetUsersListAsync();
        }
        
        /// <summary>
        /// Создать пользователя
        /// </summary>
        private async Task CreateUserAsync()
        {
            if (UsersService == null) return;

            var createDialog = ShowCreateDialog();
            if (createDialog == null) return;

            var result = await createDialog.Result;
            if (result.Cancelled) return;
            
            var dialogProcess = ShowProcessDialog("processing ...");

            // TODO: удалить
            await Task.Delay(1000);

            var (error, account) = await UsersService.CreateAccount((CreateAccountDto) result.Data);

            dialogProcess?.Close();

            IDialogReference? dialogMessage;
            
            if (error != 0 || account == null)
            {
                dialogMessage = ShowErrorDialog("Ошибка","Невозможно создать нового пользователя <br />" +
                                                         $"Код ошибки {error}");
                if (dialogMessage == null)
                    return;
                await dialogMessage.Result;
                return;
            }

            dialogMessage = ShowSuccessDialog("Создание пользователя","Новый пользователь успешно создан.");
            if (dialogMessage == null)
                return;
            await dialogMessage.Result;

            _itemsChanged = true;
            await GetUsersListAsync();
        }

        /// <summary>
        /// Показать диалог - успешное выполнение
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private IDialogReference? ShowSuccessDialog(string title, string message)
        {
            DialogOptions options = new() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true};
            var parameters = new DialogParameters
            {
                {"ContentText", message},
                {"ButtonText", "Close"},
                {"Color", Color.Default}
            };
            return DialogService?.Show<DialogLoginPage>(title, parameters, options);
        }
        
        /// <summary>
        /// Диалоговое окно создания пользователя
        /// </summary>
        /// <returns></returns>
        private IDialogReference? ShowCreateDialog()
        {
            DialogOptions options = new() { MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
            var parameters = new DialogParameters { ["SubmitButtonName"] = "Создать"};
            return DialogService?.Show<DialogCreateUser>("Создать нового пользователя", parameters, options);
        }
        
        /// <summary>
        /// Диалоговое окно удаления пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private IDialogReference? ShowDeleteDialog(AccountDto user)
        {
            DialogOptions options = new DialogOptions() { MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
            var parameters = new DialogParameters { ["User"] = user };
            return DialogService?.Show<DialogDeleteUser>("Delete User", parameters, options);
        }
        
        /// <summary>
        /// Диалоговое окно редактирования пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private IDialogReference? ShowEditDialog(AccountDto user)
        {
            DialogOptions options = new() { MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
            var parameters = new DialogParameters { ["SubmitButtonName"] = "Редактировать", ["User"] = user };
            return DialogService?.Show<DialogEditUser>("Редактирование пользователя", parameters, options);
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

        /// <summary>
        /// Показать диалог изменения пароля
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private IDialogReference? ShowChangePasswordDialog(ChangePasswordViewModel user)
        {
            DialogOptions options = new() { MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
            var parameters = new DialogParameters
            {
                ["SubmitButtonName"] = "Изменить пароль",
                ["ChangePasswordViewModel"] = user
            };
            return DialogService?.Show<DialogChangeUserPassword>("Изменить пароль", parameters, options);
        }
        
        /// <summary>
        /// Показать Processing диалог
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private IDialogReference? ShowProcessDialog(string message)
        {
            DialogOptions options = new() { DisableBackdropClick = true };
            var parameters = new DialogParameters {{"ContentText", message}};
            return DialogService?.Show<DialogProcess>("In progress", parameters, options);
        }
        
        /// <summary>
        /// Показать / скрыть детали выбранного пользователя в таблице
        /// </summary>
        /// <param name="id"></param>
        private void DetailsButtonPress(Guid id)
        {
            var user = _usersList.FirstOrDefault(u => u.User.Id == id);
            if (user != null) user.ShowDetails = !user.ShowDetails;
        }
    }

    public class UserViewModel
    {
        public bool ShowDetails { get; set; }
        public AccountDto User { get; set; } = new();
    }

    public class ChangePasswordViewModel
    {
        [StringLength(256, MinimumLength = 8)]
        public string Password { get; set; } = "";
        
        public AccountDto User { get; set; } = new();
    }
}
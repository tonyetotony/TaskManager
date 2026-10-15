using System.Windows;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class UserInfoPageViewModel : BindableBase
{
    private AuthToken _token;
    private UsersRequestService _usersRequestService;
    private CommonViewService _viewService;

    public UserModel CurrentUser { get; private set; }

    public DelegateCommand OpenEditProfileCommand { get; private set; }

    public UserInfoPageViewModel(AuthToken token)
    {
        _token = token;
        _usersRequestService = new UsersRequestService();
        _viewService = new CommonViewService();

        CurrentUser = _usersRequestService.GetCurrentUser(_token);

        OpenEditProfileCommand = new DelegateCommand(OpenEditProfile);
    }

    private void OpenEditProfile()
    {
        var selectedUser = _usersRequestService.GetUserById(_token, CurrentUser.Id);
        var viewModel = new UsersPageViewModel(_token)
        {
            SelectedUser = selectedUser,
            TypeActionWithUser = ClientAction.Update
        };

        var wnd = new CreateOrUpdateUserWindow();
        wnd.DataContext = viewModel;
        
        // Передаём окно в viewModel для закрытия после сохранения
        viewModel.WindowToClose = wnd;
        
        // Подписываемся на событие закрытия окна
        wnd.Closed += (s, e) =>
        {
            // Перезагружаем данные текущего пользователя
            CurrentUser = _usersRequestService.GetCurrentUser(_token);
            RaisePropertyChanged(nameof(CurrentUser));
        };
        
        _viewService.OpenWindow(wnd, viewModel);
    }
}

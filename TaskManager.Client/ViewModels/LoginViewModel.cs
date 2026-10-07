using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Newtonsoft.Json;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views;
using TaskManager.Common.Models;
using Path = System.IO.Path;

namespace TaskManager.Client.ViewModels;

public class LoginViewModel : BindableBase
{
    
    UsersRequestService _usersRequestService;
    
    #region COMMANDS

    public DelegateCommand<object> GetUserFromDBCommand { get; private set; }
    public DelegateCommand<object> LoginFromCacheCommand { get; private set; }

    #endregion

    public LoginViewModel()
    {
        _usersRequestService  = new UsersRequestService();
        CurrentUserCache = GetUserCache();
        GetUserFromDBCommand = new DelegateCommand<object>(GetUserFromDB);
        LoginFromCacheCommand =  new DelegateCommand<object>(LoginFromCache);
    }

    #region PROPERTIES

    private string _cachePath = Path.GetTempPath() + "usertaskmanager.txt";
    private Window _currentWindow;
    public string UserLogin { get; set; }
    public string UserPassword { get; set; }

    private UserCache _currentUserCache;

    public UserCache CurrentUserCache
    {
        get => _currentUserCache;
        set
        {
            _currentUserCache = value;
            RaisePropertyChanged(nameof(CurrentUserCache));
        }
    }
    
    private UserModel _currentUser;
    public UserModel CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            RaisePropertyChanged(nameof(CurrentUser));
        }
    }
    
    private AuthToken _authToken;
    public AuthToken AuthToken
    {
        get => _authToken;
        set
        {
            _authToken = value;
            RaisePropertyChanged(nameof(AuthToken));
        }
    }

    #endregion

    #region METHODS

    private void GetUserFromDB(object parameter)
    {
        var passBox = parameter as PasswordBox;
        bool isNewUser = false;
        _currentWindow = Window.GetWindow(passBox);
        
        if(UserLogin != CurrentUserCache?.Login || UserPassword != CurrentUserCache?.Password)
            isNewUser = true;
        
        UserPassword = passBox.Password;
        
        try
        {
            AuthToken = _usersRequestService.GetToken(UserLogin, UserPassword);
            if (AuthToken == null)
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            CurrentUser = _usersRequestService.GetCurrentUser(AuthToken);
            if (CurrentUser != null)
            {
                if (isNewUser)
                {
                    var saveUserCacheMessage = MessageBox.Show("Хотите сохранить логин и пароль?", "Сохрпанение данных",
                                MessageBoxButton.YesNo);
                    if (saveUserCacheMessage == MessageBoxResult.Yes)
                    {
                        UserCache newUserCache = new UserCache
                        {
                            Login = UserLogin,
                            Password = UserPassword
                        };
                        CreateUserCache(newUserCache);
                    }
                }
                OpenMainWindow();
            }
            else
            {
                MessageBox.Show("Пользователь не найден", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка подключения к серверу: {ex.Message}\n\nУбедитесь, что сервер запущен на {CommonRequestService.HOST}", "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CreateUserCache(UserCache userCache)
    {
        string jsonUserCache = JsonConvert.SerializeObject(userCache);
        using (StreamWriter sw = new StreamWriter(_cachePath, false, Encoding.Default))
        {
            sw.Write(jsonUserCache);
            MessageBox.Show("User cache created successfully");
        }
    }

    private UserCache GetUserCache()
    {
        bool isCacheExist = File.Exists(_cachePath);
        if (isCacheExist && File.ReadAllText(_cachePath).Length > 0)
        {
            return JsonConvert.DeserializeObject<UserCache>(File.ReadAllText(_cachePath));
        }
        return null;
    }

    private void LoginFromCache(object window)
    {
        _currentWindow = window as Window;
        UserLogin = CurrentUserCache.Login;
        UserPassword = CurrentUserCache.Password;
        AuthToken = _usersRequestService.GetToken(UserLogin, UserPassword);
        
        CurrentUser = _usersRequestService.GetCurrentUser(AuthToken);
        if (CurrentUser != null)
        {
            OpenMainWindow();
        }
    }

    private void OpenMainWindow()
    {
        MainWindow window = new MainWindow();
        window.DataContext = new MainWindowViewModel(AuthToken, CurrentUser, window);
        window.Show();
        _currentWindow.Close();
    }
    

    #endregion
    
}
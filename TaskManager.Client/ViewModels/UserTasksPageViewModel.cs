using System.Collections.ObjectModel;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.Pages;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class UserTasksPageViewModel : BindableBase
{
    private AuthToken _token;
    private TasksRequestService _tasksRequestService;
    private UsersRequestService _usersRequestService;
    private DesksRequestService _desksRequestService;
    private CommonViewService _viewService;
    private MainWindowViewModel _mainWindowVM;
    
    public UserTasksPageViewModel(AuthToken token, MainWindowViewModel mainWindowVM)
    {
        _token = token;
        _tasksRequestService = new TasksRequestService();
        _usersRequestService = new UsersRequestService();
        _desksRequestService = new DesksRequestService();
        _viewService = new CommonViewService();
        _mainWindowVM = mainWindowVM;
        
        LoadTasks();
        NavigateToDeskCommand = new DelegateCommand<object>(NavigateToDesk);
    }

    private List<TaskClient> _allTasks;
    public List<TaskClient> AllTasks
    {
        get => _allTasks ??= LoadTasks();
    }
    
    private List<TaskClient> LoadTasks()
    {
        var tasks = _tasksRequestService.GetTasksForCurrentUser(_token);
        if (tasks == null || tasks.Count == 0)
            return new List<TaskClient>();
            
        var taskClients = new List<TaskClient>();
        foreach (var task in tasks)
        {
            var taskClient = new TaskClient(task);
            if (task.CreatorId != null)
            {
                int creatorId = (int)task.CreatorId;
                taskClient.Creator = _usersRequestService.GetUserById(_token, creatorId);
            }
            if (task.ExecutorId != null)
            {
                int executorId = (int)task.ExecutorId;
                taskClient.Executor = _usersRequestService.GetUserById(_token, executorId);
            }
            taskClients.Add(taskClient);
        }
        return taskClients;
    }
    
    public DelegateCommand<object> NavigateToDeskCommand { get; private set; }
    
    public UserTasksPageViewModel() : this(new AuthToken(), null) { }
    
    public void NavigateToDesk(object deskId)
    {
        try
        {
            int deskIdInt = Convert.ToInt32(deskId);
            var desk = _desksRequestService.Get(_token, deskIdInt);
            
            if (desk == null)
            {
                _viewService?.ShowMessage("Доска не найдена");
                return;
            }
            
            var page = new ProjectDesksPage();
            _mainWindowVM?.OpenPage(page, $"Доска: {desk.Name}", 
                new ProjectDesksPageViewModel(_token, new ProjectModel { Id = desk.ProjectId }, _mainWindowVM));
        }
        catch (Exception ex)
        {
            _viewService?.ShowMessage($"Ошибка при переходе: {ex.Message}");
        }
    }
}
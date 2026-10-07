using System.Collections.ObjectModel;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class UserTasksPageViewModel : BindableBase
{
    private AuthToken _token;
    private TasksRequestService _tasksRequestService;
    private UsersRequestService _usersRequestService;
    
    public UserTasksPageViewModel(AuthToken token)
    {
        _token = token;
        _tasksRequestService = new TasksRequestService();
        _usersRequestService = new UsersRequestService();
        
        LoadTasks();
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
}
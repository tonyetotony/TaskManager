using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Client.Views.Components;
using TaskManager.Client.Views.Pages;
using TaskManager.Common.Models;
using static System.Windows.Media.VisualTreeHelper;

namespace TaskManager.Client.ViewModels;

public class DeskTasksPageViewModel : BindableBase
{
    private AuthToken _token;
    private DeskModel _desk;
    private UsersRequestService _usersRequestService;
    private TasksRequestService _tasksRequestService;
    private CommonViewService _viewService;
    private ProjectRequestService _projectRequestService;

    private DeskTasksPage _page;

    public UserModel CurrentUser
    {
        get => _usersRequestService.GetCurrentUser(_token);
    }

    #region COMMANDS

    public DelegateCommand OpenNewTaskCommand { get; private set; }
    public DelegateCommand OpenUpdateTaskCommand { get; private set; }
    public DelegateCommand CreateOrUpdateTaskCommand { get; private set; }
    public DelegateCommand DeleteTaskCommand { get; private set; }

    #endregion
    

    public DeskTasksPageViewModel(AuthToken token,  DeskModel desk, DeskTasksPage page)
    {
        _token = token;
        _desk = desk;
        _page = page;
        
        _projectRequestService = new ProjectRequestService();
        _viewService = new CommonViewService();
        _usersRequestService = new UsersRequestService();
        _tasksRequestService = new TasksRequestService();
        
        TasksByColumns = GetTasksByColumns(_desk.Id);
        _page.TasksGrid.Children.Add(CreateTasksGrid());
        
        OpenNewTaskCommand = new DelegateCommand(OpenNewTask);
        OpenUpdateTaskCommand = new DelegateCommand(OpenUpdateTask);
        CreateOrUpdateTaskCommand = new DelegateCommand(CreateOrUpdateTask);
        DeleteTaskCommand = new DelegateCommand(DeleteTask);
    }

    #region PROPERTIES

    private Dictionary<string, List<TaskClient>> _tasksByColumns =  new Dictionary<string, List<TaskClient>>();

    public Dictionary<string, List<TaskClient>> TasksByColumns
    {
        get => _tasksByColumns;
        set
        {
            _tasksByColumns = value;
            RaisePropertyChanged(nameof(TasksByColumns));
        }
    }
    
    private TaskClient _selectedTask;

    public TaskClient SelectedTask
    {
        get => _selectedTask;
        set
        {
            _selectedTask = value;
            RaisePropertyChanged(nameof(SelectedTask));
        }
        
    }
    
    private ClientAction _typeActionWithTask;

    public ClientAction TypeActionWithTask
    {
        get => _typeActionWithTask;
        set
        {
            _typeActionWithTask = value;
            RaisePropertyChanged(nameof(TypeActionWithTask));
        }
    }

    private UserModel _selectedTaskExecutor;

    public UserModel SelectedTaskExecutor
    {
        get => _selectedTaskExecutor;
        set
        {
            _selectedTaskExecutor = value;
            RaisePropertyChanged(nameof(SelectedTaskExecutor));
        }
    }

    private ProjectModel Project
    {
        get => _projectRequestService.GetProjectById(_token, _desk.ProjectId);
    }
    
    public List<UserModel> AllProjectUsers
    {
        get => Project?.AllUsersIds?.Select(userId => _usersRequestService.GetUserById(_token, userId)).ToList();
    }

    #endregion

    #region METHODS

    private Dictionary<string, List<TaskClient>> GetTasksByColumns(int deskId)
    {
        var tasksByColumns = new Dictionary<string, List<TaskClient>>();
        var allTasks = _tasksRequestService.GetTasksByDesk(_token, deskId);
        foreach (var column in _desk.Columns)
        {
            tasksByColumns.Add(column, allTasks.Where(t => t.Column == column)
                .Select(t =>
                {
                    var task = new TaskClient(t);
                    task.Creator = _usersRequestService.GetCurrentUser(_token);
                    if (t.ExecutorId.HasValue)
                    {
                        task.Executor = _usersRequestService.GetUserById(_token, t.ExecutorId.Value);
                    }
                    return task;
                }).ToList());
        }
        return tasksByColumns;
    }

    private void CreateOrUpdateTask()
    {
        if (TypeActionWithTask == ClientAction.Create)
        {
            CreateTask();
        }
        else if (TypeActionWithTask == ClientAction.Update)
        {
            UpdateTask();
        }
    }

    private void CreateTask()
    {
        if (SelectedTask == null) return;
        
        
        if (string.IsNullOrWhiteSpace(SelectedTask.Model.Name))
        {
            _viewService.ShowMessage("Введите название задачи");
            return;
        }
        
        SelectedTask.Model.DeskId = _desk.Id;
        SelectedTask.Model.ExecutorId = SelectedTaskExecutor?.Id;
        SelectedTask.Model.Column = _desk.Columns.FirstOrDefault();
        
        var resultAction = _tasksRequestService.Create(_token, SelectedTask.Model);
        if (resultAction == System.Net.HttpStatusCode.OK)
        {
            _viewService.ShowActionResult(resultAction, "Задача создана");
            UpdatePage();
        }
        else
        {
            _viewService.ShowMessage("Ошибка при создании задачи");
        }
    }
    
    private void UpdateTask()
    {
        if (SelectedTask == null) return;
        
        if (SelectedTaskExecutor != null)
            SelectedTask.Model.ExecutorId = SelectedTaskExecutor.Id;
        
        var result = _tasksRequestService.Update(_token, SelectedTask.Model);
        if (result == System.Net.HttpStatusCode.OK)
        {
            _viewService.ShowMessage("Задача обновлена");
            UpdatePage();
        }
        else
        {
            _viewService.ShowMessage("Ошибка при обновлении задачи");
        }
    }
    
    private void DeleteTask()
    {
        if (SelectedTask == null) return;
        
        var result = MessageBox.Show(
            $"Вы действительно хотите удалить задачу '{SelectedTask.Model.Name}'?",
            "Confirm deletion",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _tasksRequestService.Delete(_token, SelectedTask.Model.Id);
            UpdatePage();
        }
    }
    
    private void TaskControl_EditRequested(object sender, TaskClient task)
    {
        SelectedTask = task;
        OpenUpdateTask();
    }
    
    private void TaskControl_DeleteRequested(object sender, TaskClient task)
    {
        SelectedTask = task;
        DeleteTask();
    }
    
    private void UpdatePage()
    {
        SelectedTask = null;
        TasksByColumns = GetTasksByColumns(_desk.Id);
        _page.TasksGrid.Children.Clear();
        _page.TasksGrid.Children.Add(CreateTasksGrid());
        _viewService.CurrentOpenedWindow?.Close();
    }

    private void ColumnBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is Border border)
        {
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(6, 150, 215));
            border.Background = new SolidColorBrush(Color.FromRgb(232, 244, 248));
        }
    }

    private void ColumnBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is Border border)
        {
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224));
            border.Background = new SolidColorBrush(Color.FromRgb(248, 249, 250));
        }
    }

    private void HandleTaskDrop(object sender, System.Windows.DragEventArgs e, string targetColumn)
    {
        ColumnBorder_Drop(sender, e, targetColumn);
    }

    private void ColumnBorder_Drop(object sender, System.Windows.DragEventArgs e, string targetColumn)
    {
        if (e.Data.GetDataPresent("TaskData"))
        {
            var taskClient = e.Data.GetData("TaskData") as TaskClient;
            if (taskClient != null)
            {
                
                if (taskClient.Model.Id == 0)
                {
                    _viewService.ShowMessage("Task ID is not set. Please save the task first.");
                    return;
                }

                var oldColumn = taskClient.Model.Column;
                
                if (oldColumn == targetColumn)
                {
                    return; 
                }

                taskClient.Model.Column = targetColumn;

                // Save to server
                var result = _tasksRequestService.Update(_token, taskClient.Model);
                if (result == System.Net.HttpStatusCode.OK)
                {
                    //_viewService.ShowMessage($"Task moved from '{oldColumn}' to '{targetColumn}'");
                    UpdatePage();
                }
                else
                {
                    _viewService.ShowMessage($"Failed to save task. Status: {result}");
                    // Revert the change
                    taskClient.Model.Column = oldColumn;
                }
            }
        }
    }

    private void OpenNewTask()
    {
        TypeActionWithTask = ClientAction.Create;
        var newTask = new TaskModel
        {
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(7)
        };
        SelectedTask = new TaskClient(newTask);
        var wnd = new CreateOrUpdateTaskWindow();
        _viewService.OpenWindow(wnd, this);
    }
    
    private void OpenUpdateTask()
    {
        if (SelectedTask == null)
        {
            _viewService.ShowMessage("Выберите задачу");
            return;
        }
        
        TypeActionWithTask = ClientAction.Update;
        SelectedTaskExecutor = SelectedTask.Executor;
        var wnd = new CreateOrUpdateTaskWindow();
        _viewService.OpenWindow(wnd, this);
    }
    
    private Grid CreateTasksGrid()
    {
        Grid grid = new Grid();
        
        var row0 = new RowDefinition();
        row0.Height = new GridLength(35);
        
        var row1 = new RowDefinition();
        row1.Height = new GridLength(1, GridUnitType.Star);
        grid.RowDefinitions.Add(row0);
        grid.RowDefinitions.Add(row1);
        
        int columnCount = 0;
        foreach (var column in TasksByColumns)
        {
            // Каждая колонка занимает равную ширину
            var col = new ColumnDefinition();
            col.Width = new GridLength(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(col);
            
            // Заголовок колонки
            TextBlock header = new TextBlock();
            header.Text = column.Key;
            header.FontSize = 16;
            header.FontWeight = FontWeights.Bold;
            header.Foreground = new SolidColorBrush(Color.FromRgb(44, 62, 80));
            header.HorizontalAlignment = HorizontalAlignment.Center;
            header.VerticalAlignment = VerticalAlignment.Center;
            header.Margin = new Thickness(0);
            
            Grid.SetRow(header, 0);
            Grid.SetColumn(header, columnCount);
            
            grid.Children.Add(header);
            
            
            var columnBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Color.FromRgb(248, 249, 250)),
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(8),
                MinHeight = 200,
                Padding = new Thickness(12),
                VerticalAlignment = VerticalAlignment.Stretch
            };

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };

            
            columnBorder.AllowDrop = true;
            columnBorder.Drop += (s, e) => HandleTaskDrop(s, e, column.Key);
            columnBorder.MouseEnter += ColumnBorder_MouseEnter;
            columnBorder.MouseLeave += ColumnBorder_MouseLeave;

            var stackPanel = new StackPanel();

            var tasksViews = new List<TaskControl>();

            foreach (var task in column.Value)
            {
                var taskView = new TaskControl(task);
                taskView.Margin = new Thickness(5);
                taskView.IsAdminOrEditor = CurrentUser.Status == UserStatus.Admin || CurrentUser.Status == UserStatus.Editor;
                taskView.EditRequested += TaskControl_EditRequested;
                taskView.DeleteRequested += TaskControl_DeleteRequested;
                tasksViews.Add(taskView);
                stackPanel.Children.Add(taskView);
            }

            scrollViewer.Content = stackPanel;
            columnBorder.Child = scrollViewer;

            Grid.SetRow(columnBorder, 1);
            Grid.SetColumn(columnBorder, columnCount);
            grid.Children.Add(columnBorder);
            
            columnCount++;
        }

        return grid;
    }

    
    
    #endregion

}
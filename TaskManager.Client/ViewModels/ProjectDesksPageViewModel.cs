using System.Collections.ObjectModel;
using System.Windows;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Client.Views.Pages;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class ProjectDesksPageViewModel : BindableBase
{
    private CommonViewService _viewService;
    private DesksRequestService _desksRequestService;
    private UsersRequestService _userRequestService;
    private DesksViewService _desksViewService;
    private MainWindowViewModel _mainWindowVM;
    
    #region COMMANDS

    public DelegateCommand OpenNewDeskCommand { get; private set; }
    public DelegateCommand<object> OpenUpdateDeskCommand { get; private set; }
    public DelegateCommand CreateOrUpdateDeskCommand { get; private set; }
    public DelegateCommand DeleteDeskCommand { get; private set; }
    public DelegateCommand SelectImageForDeskCommand { get; private set; }
    public DelegateCommand AddNewColumnItemCommand { get; private set; }
    public DelegateCommand<object> RemoveColumnItemCommand { get; private set; }
    public DelegateCommand<object> OpenDeskTasksPageCommand { get; private set; }

    #endregion

    public ProjectDesksPageViewModel(AuthToken token, ProjectModel project, MainWindowViewModel mainWindowVM)
    {
        _token = token;
        _project = project;
        _mainWindowVM = mainWindowVM;
        
        _desksRequestService = new DesksRequestService();
        _viewService = new CommonViewService();
        _userRequestService = new UsersRequestService();
        _desksViewService = new DesksViewService(_token, _desksRequestService);
        
        UpdatePage();

        OpenNewDeskCommand = new DelegateCommand(OpenNewDesk);
        OpenUpdateDeskCommand  = new DelegateCommand<object>(OpenUpdateDesk);
        CreateOrUpdateDeskCommand = new DelegateCommand(CreateOrUpdateDesk);
        DeleteDeskCommand = new DelegateCommand(DeleteDesk);
        SelectImageForDeskCommand = new DelegateCommand(SelectImageForDesk);
        AddNewColumnItemCommand = new DelegateCommand(AddNewColumnItem);
        RemoveColumnItemCommand = new DelegateCommand<object>(RemoveColumnItem);
        OpenDeskTasksPageCommand = new DelegateCommand<object>(OpenDeskTasksPage);

    }

    #region PROPERTIES

 
    private AuthToken _token;
    private ProjectModel _project;

    public UserModel CurrentUser
    {
        get => _userRequestService.GetCurrentUser(_token);
    }
    
    private List<ModelClient<DeskModel>> _projectDesks;

    public List<ModelClient<DeskModel>> ProjectDesks
    {
        get => _projectDesks;
        set
        {
            _projectDesks = value;
            RaisePropertyChanged(nameof(ProjectDesks));
        }
    }
    
    private ClientAction _typeActionWithDesk;

    public ClientAction TypeActionWithDesk
    {
        get => _typeActionWithDesk;
        set
        {
            _typeActionWithDesk = value;
            RaisePropertyChanged(nameof(TypeActionWithDesk));
            RaisePropertyChanged(nameof(DeleteButtonVisibility));
        }
    }
    
    public Visibility DeleteButtonVisibility
    {
        get => TypeActionWithDesk == ClientAction.Create ? Visibility.Collapsed : Visibility.Visible;
    }
    
    private ModelClient<DeskModel> _selectedDesk;

    public ModelClient<DeskModel> SelectedDesk
    {
        get => _selectedDesk;
        set
        {
            _selectedDesk = value;
            RaisePropertyChanged(nameof(SelectedDesk));
            
        }
    }

    private ObservableCollection<ColumnBindingHelp> _columnsForNewDesks = new ObservableCollection<ColumnBindingHelp>()
    {
        new ColumnBindingHelp("Новые"),
        new ColumnBindingHelp("В работе"),
        new ColumnBindingHelp("На проверке"),
        new ColumnBindingHelp("Выполненные")

    };

    public ObservableCollection<ColumnBindingHelp> ColumnsForNewDesk
    {
        get => _columnsForNewDesks;
        set
        {
            _columnsForNewDesks = value;
            RaisePropertyChanged(nameof(ColumnsForNewDesk));
                
        }
    }

    #endregion
    
    

    private void OpenNewDesk()
    {
        SelectedDesk = new ModelClient<DeskModel>(new DeskModel());
        TypeActionWithDesk = ClientAction.Create;
        
        var wnd = new CreateOrUpdateDeskWindow();
        _viewService.OpenWindow(wnd, this);
    }
    
    private void OpenUpdateDesk(object deskId)
    { 
        SelectedDesk = _desksViewService.GetDeskClientById(deskId);
        if (CurrentUser.Id != SelectedDesk.Model.AdminId)
        {
           _viewService.ShowMessage("Операция доступна только администратору!");
            return; 
        }
 
        TypeActionWithDesk = ClientAction.Update;
        ColumnsForNewDesk = new ObservableCollection<ColumnBindingHelp>(SelectedDesk.Model.Columns.Select(c=>new ColumnBindingHelp(c)));
        
        _desksViewService.OpenViewDeskInfo(deskId, this);
    }
    
    private void CreateOrUpdateDesk()
    {
        if (SelectedDesk == null || SelectedDesk.Model == null)
        {
            _viewService.ShowMessage("Desk data is not available");
            return;
        }
        
        if (TypeActionWithDesk == ClientAction.Create)
        {
            CreateDesk();
            
        }
        else if (TypeActionWithDesk == ClientAction.Update)
        {
            UpdateDesk();
            UpdatePage();
        }
    }
    
    private void CreateDesk()
    {
        
        if (string.IsNullOrWhiteSpace(SelectedDesk.Model.Name))
        {
            MessageBox.Show("Введите название доски", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        if (ColumnsForNewDesk == null || ColumnsForNewDesk.Count == 0)
        {
            MessageBox.Show("Добавьте хотя бы одну колонку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        
        var columnNames = ColumnsForNewDesk.Select(c => c.Value).ToList();
        if (columnNames.All(string.IsNullOrWhiteSpace))
        {
            MessageBox.Show("Заполните названия хотя бы одной колонки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        SelectedDesk.Model.Columns = columnNames.ToArray();
        SelectedDesk.Model.ProjectId = _project.Id;
        SelectedDesk.Model.AdminId = _userRequestService.GetCurrentUser(_token).Id;
        
        var resultAction = _desksRequestService.Create(_token, SelectedDesk.Model);
        if (resultAction == System.Net.HttpStatusCode.OK)
        {
            _viewService.ShowMessage("Доска успешно создана");
            UpdatePage();
        }
        else
        {
            _viewService.ShowMessage("Ошибка при создании доски. Проверьте данные.");
        }
    }
    
    private void UpdateDesk()
    {
        SelectedDesk.Model.Columns = ColumnsForNewDesk.Select(c => c.Value).ToArray();
        _desksViewService.UpdateDesk(SelectedDesk.Model);
    }
    
    private void DeleteDesk()
    {
        if (SelectedDesk?.Model?.Id > 0)
        {
            var result = MessageBox.Show(
                $"Вы действительно хотите удалить доску '{SelectedDesk.Model.Name}'?",
                "Confirm deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _desksViewService.DeleteDesk(SelectedDesk.Model.Id);
                UpdatePage();
            }
        }
        else
        {
            _viewService.ShowMessage("Desk ID is not available");
        }
    }
    
    private void UpdatePage()
    {
        SelectedDesk = null;
        ProjectDesks = _desksViewService.GetDesks(_project.Id);
        _viewService.CurrentOpenedWindow?.Close();
    }
    
    private void SelectImageForDesk()
    {
        SelectedDesk = _desksViewService.SelectImageForDesk(SelectedDesk);
    }

    private void AddNewColumnItem()
    {
        ColumnsForNewDesk.Add(new ColumnBindingHelp("Новая колонка"));
    }
    
    private void RemoveColumnItem(object item)
    {
        var itemToRemove = (ColumnBindingHelp)item;
        ColumnsForNewDesk.Remove(itemToRemove);
    }

    private void OpenDeskTasksPage(object deskId)
    {
        SelectedDesk = _desksViewService.GetDeskClientById(deskId);
        var page = new DeskTasksPage();
        var context = new DeskTasksPageViewModel(_token, SelectedDesk.Model, page);
        _mainWindowVM.OpenPage(page, $"Задача {SelectedDesk.Model.Name}",context);
    }
    
}
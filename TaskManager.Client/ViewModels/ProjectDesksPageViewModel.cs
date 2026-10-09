using System.Collections.ObjectModel;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class ProjectDesksPageViewModel : BindableBase
{
    private CommonViewService _viewService;
    private DesksRequestService _desksRequestService;
    #region COMMANDS

    public DelegateCommand OpenNewDeskCommand { get; private set; }
    public DelegateCommand<object> OpenUpdateDeskCommand { get; private set; }
    public DelegateCommand CreateOrUpdateDeskCommand { get; private set; }
    public DelegateCommand DeleteDeskCommand { get; private set; }
    public DelegateCommand SelectImageForDeskCommand { get; private set; }

    #endregion

    public ProjectDesksPageViewModel(AuthToken token, ProjectModel project)
    {
        _token = token;
        _project = project;
        _desksRequestService = new DesksRequestService();
        _viewService = new CommonViewService();
        UpdatePage();

        OpenNewDeskCommand = new DelegateCommand(OpenNewDesk);
        OpenUpdateDeskCommand  = new DelegateCommand<object>(OpenUpdateDesk);
        CreateOrUpdateDeskCommand = new DelegateCommand(CreateOrUpdateDesk);
        DeleteDeskCommand = new DelegateCommand(DeleteDesk);
        SelectImageForDeskCommand = new DelegateCommand(SelectImageForDesk);
    }

    #region PROPERTIES

 
    private AuthToken _token;
    private ProjectModel _project;
    
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
        }
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
        new ColumnBindingHelp("New"),
        new ColumnBindingHelp("In progress"),
        new ColumnBindingHelp("In review"),
        new ColumnBindingHelp("Completed")

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
    
    private List<ModelClient<DeskModel>> GetDesks(int projectId)
    {
        var result = new List<ModelClient<DeskModel>>();
        var desks = _desksRequestService.GetProjectDesks(_token, _project.Id);
        if (desks != null)
        {
            result = desks.Select(d => new ModelClient<DeskModel>(d)).ToList();
        }

        return result;
    }

    private void OpenNewDesk()
    {
        SelectedDesk = new ModelClient<DeskModel>(new DeskModel());
        TypeActionWithDesk = ClientAction.Create;
        
        var wnd = new CreateOrUpdateDeskWindow();
        _viewService.OpenWindow(wnd, this);
    }
    
    private void OpenUpdateDesk(object deskId)
    { 
        SelectedDesk = GetDeskClientById(deskId);
        TypeActionWithDesk = ClientAction.Update;
        
        var wnd = new CreateOrUpdateDeskWindow();
        _viewService.OpenWindow(wnd, this);
    }
    
    private void CreateOrUpdateDesk()
    {
        if (TypeActionWithDesk == ClientAction.Create)
        {
            CreateDesk();   
        }
        if (TypeActionWithDesk == ClientAction.Update)
        {
            UpdateDesk();
        }
        UpdatePage();
        
    }
    
    private void CreateDesk()
    {
        var resultAction = _desksRequestService.Create(_token, SelectedDesk.Model);
        _viewService.ShowActionResult(resultAction, "Desk is created");
    }
    
    private void UpdateDesk()
    {
        var resultAction = _desksRequestService.Update(_token, SelectedDesk.Model);
        _viewService.ShowActionResult(resultAction, "Desk is updated");
    }
    
    private void DeleteDesk()
    {
        var resultAction = _desksRequestService.Delete(_token, SelectedDesk.Model.Id);
        _viewService.ShowActionResult(resultAction, "Desk is deleted");
        UpdatePage();
    }
    
    private void UpdatePage()
    {
        SelectedDesk = null;
        ProjectDesks = GetDesks(_project.Id);
    }
    
    private void SelectImageForDesk()
    {
        _viewService.SetPhotoForObject(SelectedDesk.Model);
        SelectedDesk = new ModelClient<DeskModel>(SelectedDesk.Model);
    }
    
    private ModelClient<DeskModel> GetDeskClientById(object deskId)
    {
        try
        {
            int id = (int)deskId;
            DeskModel desk = _desksRequestService.Get(_token, id);
            return new ModelClient<DeskModel>(desk);
        }
        catch (FormatException)
        {
            return new ModelClient<DeskModel>(null);
        }
        
    }
}
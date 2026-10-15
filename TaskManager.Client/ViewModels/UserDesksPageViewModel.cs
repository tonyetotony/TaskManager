using System.Collections.ObjectModel;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class UserDesksPageViewModel : BindableBase
{
    private AuthToken _token;
    private CommonViewService _viewService;
    private DesksRequestService _desksRequestService;
    private DesksViewService _desksViewService;

    #region COMMANDS

    public DelegateCommand<object> OpenEditDeskCommand { get; private set; }
    public DelegateCommand CreateOrUpdateDeskCommand { get; private set; }
    public DelegateCommand<object> DeleteDeskCommand { get; private set; }
    public DelegateCommand SelectImageForDeskCommand { get; private set; }
    public DelegateCommand AddNewColumnItemCommand { get; private set; }
    public DelegateCommand<object> RemoveColumnItemCommand { get; private set; }

    #endregion
    
    public UserDesksPageViewModel(AuthToken token)
    {
        _token = token;
        _desksRequestService = new DesksRequestService();
        _viewService = new CommonViewService();
        _desksViewService = new DesksViewService(_token, _desksRequestService);

        OpenEditDeskCommand = new DelegateCommand<object>(OpenUpdateDesk);
        CreateOrUpdateDeskCommand = new DelegateCommand(UpdateDesk);
        DeleteDeskCommand = new DelegateCommand<object>(DeleteDesk);
        SelectImageForDeskCommand = new DelegateCommand(SelectImageForDesk);
        
        AddNewColumnItemCommand = new DelegateCommand(AddNewColumnItem);
        RemoveColumnItemCommand = new DelegateCommand<object>(RemoveColumnItem);
        
        ContextMenuCommands.Add("Edit", OpenEditDeskCommand);
        ContextMenuCommands.Add("Delete", DeleteDeskCommand);
        
        UpdatePage();

    }

    #region PROPERTIES

    public UserModel CurrentUser
    {
        get => new UsersRequestService().GetCurrentUser(_token);
    }

    private List<ModelClient<DeskModel>> _allDesks = new List<ModelClient<DeskModel>>();
    public List<ModelClient<DeskModel>> AllDesks
    {
        get => _allDesks;
        set
        {
            _allDesks = value;
            RaisePropertyChanged(nameof(AllDesks));
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
    #endregion

    #region METHODS

    

    
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
    
    private void OpenUpdateDesk(object deskId)
    { 
        if (deskId == null)
        {
            _viewService.ShowMessage("Desk ID is not available");
            return;
        }
        
        SelectedDesk = _desksViewService.GetDeskClientById(deskId);
        if (SelectedDesk != null && SelectedDesk.Model != null)
        {
            ColumnsForNewDesk = new ObservableCollection<ColumnBindingHelp>(SelectedDesk.Model.Columns.Select(c=>new ColumnBindingHelp(c)));
            _desksViewService.OpenViewDeskInfo(deskId, this);
        }
        else
        {
            _viewService.ShowMessage("Failed to load desk data");
        }
    }

    private void UpdateDesk()
    {
        if (SelectedDesk?.Model == null)
        {
            _viewService.ShowMessage("Desk data is not available");
            return;
        }
        
        SelectedDesk.Model.Columns = ColumnsForNewDesk.Select(c => c.Value).ToArray();
        _desksViewService.UpdateDesk(SelectedDesk.Model);
        UpdatePage();
    }
    
    private void DeleteDesk(object deskId)
    {
        if (deskId != null)
        {
            _desksViewService.DeleteDesk((int)deskId);
            UpdatePage();
        }
    }
    
    
    private Dictionary<string, DelegateCommand<object>> _contextMenuCommands =  new Dictionary<string, DelegateCommand<object>>();

    public Dictionary<string, DelegateCommand<object>> ContextMenuCommands
    {
        get => _contextMenuCommands;
        set
        {
            _contextMenuCommands = value;
            RaisePropertyChanged(nameof(ContextMenuCommands));
        }
    }
    
    private void SelectImageForDesk()
    {
        SelectedDesk = _desksViewService.SelectImageForDesk(SelectedDesk);
    }
    
    private void UpdatePage()
    {
        SelectedDesk = null;
        AllDesks = _desksRequestService.GetDesksForCurrentUser(_token).Select(desk => new ModelClient<DeskModel>(desk)).ToList();
        _viewService.CurrentOpenedWindow?.Close();
    }
    
    private void AddNewColumnItem()
    {
        ColumnsForNewDesk.Add(new ColumnBindingHelp("New column"));
    }
    
    private void RemoveColumnItem(object item)
    {
        var itemToRemove = (ColumnBindingHelp)item;
        ColumnsForNewDesk.Remove(itemToRemove);
    }

    #endregion
    
}
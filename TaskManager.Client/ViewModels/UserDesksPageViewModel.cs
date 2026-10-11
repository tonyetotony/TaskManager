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

    public DelegateCommand OpenEditDeskCommand { get; private set; }
    public DelegateCommand CreateOrUpdateDeskCommand { get; private set; }
    public DelegateCommand DeleteDeskCommand { get; private set; }
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

        OpenEditDeskCommand = new DelegateCommand(OpenUpdateDesk);
        CreateOrUpdateDeskCommand = new DelegateCommand(UpdateDesk);
        DeleteDeskCommand = new DelegateCommand(DeleteDesk);
        SelectImageForDeskCommand = new DelegateCommand(SelectImageForDesk);
        
        AddNewColumnItemCommand = new DelegateCommand(AddNewColumnItem);
        RemoveColumnItemCommand = new DelegateCommand<object>(RemoveColumnItem);
        
        ContextMenuCommands.Add("Edit", OpenEditDeskCommand);
        ContextMenuCommands.Add("Delete", DeleteDeskCommand);
        
        UpdatePage();

    }

    #region PROPERTIES

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
    
    private void OpenUpdateDesk()
    { 
        SelectedDesk = _desksViewService.GetDeskClientById(SelectedDesk.Model.Id);
        ColumnsForNewDesk = new ObservableCollection<ColumnBindingHelp>(SelectedDesk.Model.Columns.Select(c=>new ColumnBindingHelp(c)));
        _desksViewService.OpenViewDeskInfo(SelectedDesk.Model.Id, this);
    }

    private void UpdateDesk()
    {
        SelectedDesk.Model.Columns = ColumnsForNewDesk.Select(c => c.Value).ToArray();
        _desksViewService.UpdateDesk(SelectedDesk.Model);
        UpdatePage();
    }
    
    private void DeleteDesk()
    {
        _desksViewService.DeleteDesk(SelectedDesk.Model.Id);
        UpdatePage();
    }
    
    
    private Dictionary<string, DelegateCommand> _contextMenuCommands =  new Dictionary<string, DelegateCommand>();

    public Dictionary<string, DelegateCommand> ContextMenuCommands
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
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class ProjectsPageViewModel : BindableBase
{
    private AuthToken _token;
    private UsersRequestService  _usersRequestService;
    private ProjectRequestService _projectRequestService;
    private CommonViewService _viewService;
    
    
    #region COMMANDS

    public DelegateCommand OpenNewProjectCommand { get; private set; }
    public DelegateCommand<object> OpenUpdateProjectCommand { get; private set; }
    public DelegateCommand<object> ShowProjectInfoCommand { get; private set; }

    #endregion
    
    
    public ProjectsPageViewModel(AuthToken token)
    {
        _viewService = new CommonViewService();
        _usersRequestService = new UsersRequestService();
        _projectRequestService = new ProjectRequestService();
        _token = token; 
        OpenNewProjectCommand = new DelegateCommand(OpenNewProject);
        OpenUpdateProjectCommand = new DelegateCommand<object>(OpenUpdateProject);
        ShowProjectInfoCommand = new DelegateCommand<object>(ShowProjectInfo);
    }

    #region PROPERTIES

    

    public List<ModelClient<ProjectModel>> UserProjects
    {
        get => _projectRequestService.GetProjects(_token).Select(project => new ModelClient<ProjectModel>(project)).ToList();
    }

    private ModelClient<ProjectModel> _selectedProject;

    public ModelClient<ProjectModel> SelectedProject
    {
        get => _selectedProject;
        set
        {
            _selectedProject = value;
            RaisePropertyChanged(nameof(SelectedProject));
            if(SelectedProject.Model.AllUsersIds != null && SelectedProject.Model.AllDesksIds.Count > 0)
                ProjectUsers = SelectedProject.Model.AllUsersIds?.Select(userId => _usersRequestService.GetUserById(_token, userId)).ToList();
            else
                ProjectUsers = new List<UserModel>();
            
        }
    }
    
    private List<UserModel> _projectUsers = new List<UserModel>();

    public List<UserModel> ProjectUsers
    {
        get => _projectUsers;
        set
        {
            _projectUsers = value;
            RaisePropertyChanged(nameof(ProjectUsers));
        }
    }
    
    #endregion

    #region METHODS

    private void OpenNewProject()
    { 
        _viewService.ShowMessage(nameof(OpenNewProject));
    }
    
    private void OpenUpdateProject(object projectId)
    { 
        SelectedProject = GetProjectClientById(projectId);
    }
    
    private void ShowProjectInfo(object projectId)
    {
        SelectedProject = GetProjectClientById(projectId);
    }


    private ModelClient<ProjectModel> GetProjectClientById(object projectId)
    {
        try
        {
            int id = (int)projectId;
            ProjectModel project = _projectRequestService.GetProjectById(_token, id);
            return new ModelClient<ProjectModel>(project);
        }
        catch (FormatException)
        {
            return new ModelClient<ProjectModel>(null);
        }
        
    }
    #endregion
    
}
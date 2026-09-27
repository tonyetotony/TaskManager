namespace TaskManager.Common.Models;

public class ProjectModel : CommonModel
{
    public ProjectStatus? Status { get; set; }
    public int? AdminId { get; set; }
    public List<int> AllUsersIds { get; set; } = new List<int>();
    public List<int> AllDesksIds { get; set; } = new List<int>();

    public ProjectModel(){}

    public ProjectModel(string projectName, string description, ProjectStatus status, int adminId)
    {
        Name = projectName;
        Description = description;
        Status = status;
        AdminId = adminId;
    }
}
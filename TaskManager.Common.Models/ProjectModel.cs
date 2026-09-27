namespace TaskManager.Common.Models;

public class ProjectModel : CommonModel
{
    public ProjectStatus? Status { get; set; }
    public int? AdminId { get; set; }
    public List<int> AllUsersIds { get; set; } = new List<int>();
    public List<int> AllDesksIds { get; set; } = new List<int>();
}
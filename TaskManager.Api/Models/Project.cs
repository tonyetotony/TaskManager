using TaskManager.Common.Models;

namespace TaskManager.Api.Models;

public class Project: CommonObject
{
    public int Id { get; set; }
    public int? AdminId { get; set; }
    public ProjectAdmin Admin { get; set; }
    public List<User> AllUsers { get; set; } = new List<User>();
    public List<Desk> AllDesks { get; set; } = new List<Desk>();
    public ProjectStatus Status { get; set; }
    public Project(){}

    public Project(ProjectModel projectModel) : base(projectModel)
    {
        AdminId = projectModel.AdminId;
        Status = projectModel.Status ?? ProjectStatus.InProgress;
    }

    public ProjectModel ToDto()
    {
        return new ProjectModel()
        {
            Id = this.Id,
            Name = this.Name,
            Description = this.Description,
            CreationDate = this.CreationDate,
            Image = this.Image,
            AdminId = this.AdminId,
            Status = this.Status

        };
    }
}
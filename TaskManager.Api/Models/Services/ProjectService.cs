using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models.Data;
using TaskManager.Common.Models;

namespace TaskManager.Api.Models.Services;

public class ProjectService : AbstractionService, ICommonService<ProjectModel>
{
    private readonly ApplicationContext _db;
    
    public ProjectService(ApplicationContext db)
    {
        _db = db;
    }
    
    public bool Create(ProjectModel model)
    {
        bool result = DoAction(delegate()
        {
            Project project = new Project(model);
            _db.Projects.Add(project);
            _db.SaveChanges();
        });
        return result;
    }

    public bool Update(int id, ProjectModel model)
    {
        bool result = DoAction(delegate()
        {
            Project project = _db.Projects.FirstOrDefault(p => p.Id == id);
            project.Name = model.Name;
            project.Description = model.Description;
            project.Image = model.Image;
            project.Status = model.Status;
            project.AdminId = model.AdminId;
            _db.Projects.Update(project);
            _db.SaveChanges();
        });
        return result;
    }

    public bool Delete(int id)
    {
        bool result = DoAction(delegate()
        {
            Project project = _db.Projects.FirstOrDefault(p => p.Id == id);
            _db.Projects.Remove(project);
            _db.SaveChanges();
        });
        return result;
    }

    public ProjectModel Get(int id)
    {
        Project project = _db.Projects.FirstOrDefault(p => p.Id == id);
        return project?.ToDto();
    }
    public async Task<IEnumerable<ProjectModel>> GetByUserId(int userId)
    {
        List<ProjectModel> result = new List<ProjectModel>();
        var admin = _db.ProjectsAdmins.FirstOrDefault(a => a.UserId == userId);
        if (admin != null)
        {
            var projectForAdmin = await _db.Projects.Where(p => p.AdminId == admin.Id)
                .Select(p => p.ToDto()).ToListAsync();
            result.AddRange(projectForAdmin);
        }
        var projectsForUser = await _db.Projects.Include(p => p.AllUsers)
            .Where(p => p.AllUsers.Any(u => u.Id == userId)).Select(p => p.ToDto()).ToListAsync();
        result.AddRange(projectsForUser);
        return result;
    }

    public IQueryable<ProjectModel> GetAll()
    {
        return _db.Projects.Select(p => p.ToDto());
    }

    public void AddUserToProject(int id, List<int> userIds)
    {
        Project project = _db.Projects.FirstOrDefault(p => p.Id == id);
        foreach (int userId in userIds)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            project.AllUsers.Add(user);
        }
        
        _db.SaveChanges();
    }
    
    public void RemoveUserFromProject(int id, List<int> userIds)
    {
        Project project = _db.Projects.Include(p => p.AllUsers).FirstOrDefault(p => p.Id == id);
        foreach (int userId in userIds)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (project.AllUsers.Contains(user))
                project.AllUsers.Remove(user);
        }
        
        _db.SaveChanges();
    }
}
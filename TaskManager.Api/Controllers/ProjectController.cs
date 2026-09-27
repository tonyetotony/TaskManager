using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Data;
using TaskManager.Api.Models.Services;
using TaskManager.Common.Models;

namespace TaskManager.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly ApplicationContext _db;
    private readonly UserService _userService;
    private readonly ProjectService _projectService;

    public ProjectController(ApplicationContext db)
    {
        _db = db;
        _userService = new UserService(db);
        _projectService = new ProjectService(db);
    }
    [HttpGet]
    public async Task<IEnumerable<CommonModel>> Get()
    {
        var user = _userService.GetUser(HttpContext.User.Identity.Name);
        if (user.Status == UserStatus.Admin)
            return await _projectService.GetAll().ToListAsync();
        else
        {
            return await _projectService.GetByUserId(user.Id); 
        }
    }
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var project = _projectService.Get(id);
        return project == null ? NoContent() : Ok(project);
    }
    [HttpPost]
    public IActionResult Create([FromBody] ProjectModel projectModel)
    {
        if (projectModel != null)
        {
            var user = _userService.GetUser(HttpContext.User.Identity.Name);
            if (user != null)
            {
                if (user.Status == UserStatus.Admin || user.Status == UserStatus.Editor)
                {
                    var admin = _db.ProjectsAdmins.FirstOrDefault(a => a.UserId == user.Id);
                    if (admin == null)
                    {
                        admin = new ProjectAdmin(user);
                        _db.ProjectsAdmins.Add(admin);
                        _db.SaveChanges();
                    }
                    projectModel.AdminId = admin.Id;
                    bool result = _projectService.Create(projectModel);
                    return result ? Ok() : NotFound();
                }
            }
            return Unauthorized();
        }
        return BadRequest();    
    }
    [HttpPatch("{id}")]
    public IActionResult Update(int id, [FromBody] ProjectModel projectModel)
    {
        if (projectModel != null)
        {
            var user = _userService.GetUser(HttpContext.User.Identity.Name);
            if (user != null)
            {
                if (user.Status == UserStatus.Admin || user.Status == UserStatus.Editor)
                {
                    bool result = _projectService.Update(id, projectModel);
                    return result ? Ok() : NotFound();
                }
                return Unauthorized();
            }
        }
        return BadRequest();
    }

    
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        bool result = _projectService.Delete(id);
        return result ? Ok() : NotFound();
    }
    
    [HttpPatch("{id}/users")]
       public IActionResult AddUsersToProject(int id, [FromBody] List<int> userIds)
       {
           if (userIds != null)
           {
               var user = _userService.GetUser(HttpContext.User.Identity.Name);
               if (user != null)
               {
                   if (user.Status == UserStatus.Admin || user.Status == UserStatus.Editor)
                   {
                       _projectService.AddUsersToProject(id, userIds);
                       return Ok();
                   }
                   return Unauthorized();
               }
           }
           return BadRequest();
       }
       
    [HttpPatch("{id}/users/remove")]
    public IActionResult RemoveUsersFromProject(int id, [FromBody] List<int> userIds)
    {
        if (userIds != null)
        {
            var user = _userService.GetUser(HttpContext.User.Identity.Name);
            if (user != null)
            {
                if (user.Status == UserStatus.Admin || user.Status == UserStatus.Editor)
                {
                    _projectService.RemoveUsersFromProject(id, userIds);
                    return Ok();
                }
                return Unauthorized();
            }
        }
        return BadRequest();
    }
}
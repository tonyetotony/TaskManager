using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models.Data;
using TaskManager.Api.Models.Services;
using TaskManager.Common.Models;

namespace TaskManager.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TasksController : ControllerBase
{
    
    private readonly ApplicationContext _db;
    private readonly UserService _userService;
    private readonly TaskService _taskService;

    public TasksController(ApplicationContext db)
    {
        _db = db;
        _userService = new UserService(db);
        _taskService = new TaskService(db);
    }

    [HttpGet]
    public async Task<IEnumerable<CommonModel>> GetTasksByDesk(int deskId)
    {
        return await _taskService.GetAll(deskId).ToListAsync();      
    }
    
    [HttpGet("user")]
    public async Task<IEnumerable<TaskModel>> GetTasksForCurrentUser()
    {
        var user = _userService.GetUser(HttpContext.User.Identity.Name);
        if (user != null)
        {
            return await _taskService.GetTasksForUser(user.Id).ToListAsync();
        }
        return Array.Empty<TaskModel>();      
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var task = _taskService.Get(id);
        return task == null ? NotFound() : Ok(task);
    }

    [HttpPost]
    public IActionResult Create([FromBody] TaskModel model)
    {
        var user = _userService.GetUser(HttpContext.User.Identity.Name);
        if (user != null)
        {
            if (model != null)
            {
                model.CreatorId = user.Id;
                bool result = _taskService.Create(model);
                return result ? Ok(model) : NotFound();
            }
            return BadRequest();
        }
        return Unauthorized();

    }
    
    [HttpPatch("{id}")]
    public IActionResult Update(int id, [FromBody] TaskModel taskModel)
    {
        var user = _userService.GetUser(HttpContext.User.Identity.Name);
        if (user != null)
        {
            if (taskModel != null)
            {
                bool result = _taskService.Update(id, taskModel);
                return result ? Ok() : NotFound();
            }
            return BadRequest();
        }

        return Unauthorized();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        bool  result = _taskService.Delete(id);
        return result ? Ok() : NotFound();
    }
    
}
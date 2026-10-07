using TaskManager.Api.Models.Data;
using TaskManager.Common.Models;

namespace TaskManager.Api.Models.Services;

public class TaskService : AbstractionService, ICommonService<TaskModel>
{
    private readonly ApplicationContext _db;
    
    public TaskService(ApplicationContext db)
    {
        _db = db;
    }
    public bool Create(TaskModel model)
    {
        bool result = DoAction(delegate()
        {
            Task newTask = new Task(model);
            _db.Tasks.Add(newTask);
            _db.SaveChanges();

            // Добавляем ID задачи в TasksIds у Desk
            var desk = _db.Desks.FirstOrDefault(d => d.Id == model.DeskId);
            if (desk != null && !desk.Tasks.Contains(newTask))
            {
                desk.Tasks.Add(newTask);
                _db.SaveChanges();
            }
        });
        return result;
    }

    public bool Update(int id, TaskModel model)
    {
        bool result = DoAction(delegate()
        {
            Task task = _db.Tasks.FirstOrDefault(t => t.Id == id);
            if(task == null) return;
            
            if (!string.IsNullOrEmpty(model.Name))
                task.Name = model.Name;
            if (!string.IsNullOrEmpty(model.Description))
                task.Description = model.Description;
            if (model.Image != null && model.Image.Length > 0)
                task.Image = model.Image;
            if (model.StartDate != default)
                task.StartDate = model.StartDate;
            if (model.EndDate != default)
                task.EndDate = model.EndDate;
            if (model.File != null && model.File.Length > 0)
                task.File = model.File;
            if (model.DeskId != 0)
                task.DeskId = model.DeskId;
            if (!string.IsNullOrEmpty(model.Column))
                task.Column = model.Column;
            if (model.CreatorId.HasValue && model.CreatorId.Value > 0)
                task.CreatorId = model.CreatorId;
            if (model.ExecutorId.HasValue && model.ExecutorId.Value > 0)
                task.ExecutorId = model.ExecutorId;
            
            _db.SaveChanges();
        });
        return result;
    }

    public bool Delete(int id)
    {
        bool result = DoAction(delegate()
        {
            Task task = _db.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return;

            // Удаляем задачу из Desk
            var desk = _db.Desks.FirstOrDefault(d => d.Id == task.DeskId);
            if (desk != null)
            {
                desk.Tasks.Remove(task);
            }

            _db.Tasks.Remove(task);
            _db.SaveChanges();
        });
        return result;
    }

    public TaskModel Get(int id)
    {
        Task task = _db.Tasks.FirstOrDefault(t => t.Id == id);
        return task?.ToDto();
    }

    public IQueryable<TaskModel> GetTasksForUser(int userId)
    {
        return _db.Tasks.Where(t=>t.CreatorId == userId || t.ExecutorId == userId).Select(t=>t.ToDto());
    }

    public IQueryable<TaskModel> GetAll(int deskId)
    {
        return _db.Tasks.Where(t=>t.DeskId == deskId).Select(t => t.ToDto());
    }
}
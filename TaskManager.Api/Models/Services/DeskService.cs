using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models.Data;
using TaskManager.Common.Models;

namespace TaskManager.Api.Models.Services;

public class DeskService : AbstractionService, ICommonService<DeskModel>
{
    private readonly ApplicationContext _db;
    
    public DeskService(ApplicationContext db)
    {
        _db = db;
    }

    public bool Create(DeskModel model)
    {
        bool result = DoAction(delegate()
        {
            Desk newDesk = new Desk(model);
            _db.Desks.Add(newDesk);
            _db.SaveChanges();
        });
        return result;
    }
    
    public bool Delete(int id)
    {
        bool result = DoAction(delegate()
        {
            Desk desk = _db.Desks.FirstOrDefault(d => d.Id == id);
            _db.Desks.Remove(desk);
            _db.SaveChanges();
        });
        return result;
    }
    
    public DeskModel Get(int id)
    {
        Desk desk = _db.Desks.Include(d => d.Tasks).FirstOrDefault(d => d.Id == id);
        return desk?.ToDto();
    }
    
    public bool Update(int id, DeskModel model)
    {
        bool result = DoAction(delegate()
        {
            Desk desk = _db.Desks.FirstOrDefault(d => d.Id == id);
            if(desk == null) return;
            
            if (!string.IsNullOrEmpty(model.Name))
                desk.Name = model.Name;
            if (!string.IsNullOrEmpty(model.Description))
                desk.Description = model.Description;
            if (model.Image != null && model.Image.Length > 0)
                desk.Image = model.Image;   
            if (model.AdminId.HasValue && model.AdminId.Value > 0) 
                desk.AdminId = model.AdminId;
            if (model.IsPrivate != model.IsPrivate)
                desk.IsPrivate = model.IsPrivate;
            if (model.ProjectId != model.ProjectId) 
                desk.ProjectId = model.ProjectId;
            desk.Columns = "[" + string.Join(",", model.Columns) + "]";
            _db.SaveChanges();
        });
        return result;
    }

    public IQueryable<CommonModel> GetAll(int userId)
    {
        var user = _db.Users.FirstOrDefault(u => u.Id == userId);
        if (user != null && user.Status == UserStatus.Admin)
            return _db.Desks.Include(d => d.Tasks).Select(d => d.ToDto() as CommonModel);
        
        return _db.Desks.Where(d=>d.AdminId == userId).Include(d => d.Tasks).Select(d => d.ToDto() as CommonModel);
    }

    public IQueryable<CommonModel> GetProjectDesks(int projectId, int userId)
    {
        var user = _db.Users.FirstOrDefault(u => u.Id == userId);
        if (user != null && user.Status == UserStatus.Admin)
            return _db.Desks.Where(d => d.ProjectId == projectId).Include(d => d.Tasks).Select(d => d.ToDto() as CommonModel);
        
        return _db.Desks.Where(d=> 
                (d.ProjectId == projectId && 
                 (d.AdminId == userId || d.IsPrivate == false)))
            .Include(d => d.Tasks)
            .Select(d => d.ToDto() as CommonModel);
    }
}
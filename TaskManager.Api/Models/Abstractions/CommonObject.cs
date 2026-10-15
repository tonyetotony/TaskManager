using TaskManager.Common.Models;

namespace TaskManager.Api.Models;

public class CommonObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    public byte[]? Image { get; set; }

    public CommonObject()
    {
        CreationDate = DateTime.Now;
        Image = null;
    }
    public CommonObject(CommonModel model)
    {
        Name = model.Name;
        Description = model.Description;
        CreationDate = model.CreationDate == default ? DateTime.Now : model.CreationDate;
        Image = model.Image ?? Array.Empty<byte>();
    }
}
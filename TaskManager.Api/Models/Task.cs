namespace TaskManager.Api.Models;

public class Task: CommonObject
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public byte[] File { get; set; }
}
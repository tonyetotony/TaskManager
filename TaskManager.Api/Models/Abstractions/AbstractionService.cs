namespace TaskManager.Api.Models;

public abstract class AbstractionService
{

    public bool DoAction(Action action)
    {
        try
        {
            action.Invoke();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] {DateTime.Now}: {e.Message}");
            Console.WriteLine(e.StackTrace);
            return false;
        }
    }
}
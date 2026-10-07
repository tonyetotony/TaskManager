using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services;

public class TasksRequestService : CommonRequestService
{
    private string _tasksControllerUrl = HOST + "tasks";
    
    public List<TaskModel> GetTasksForCurrentUser(AuthToken token)
    {
        try
        {
            string responce = GetDataByUrl(HttpMethod.Get, _tasksControllerUrl + "/user", token);
            if (string.IsNullOrEmpty(responce))
                return new List<TaskModel>();
                
            List<TaskModel> tasks = JsonConvert.DeserializeObject<List<TaskModel>>(responce);
            return tasks ?? new List<TaskModel>();
        }
        catch
        {
            return new List<TaskModel>();
        }
    }
    
    public TaskModel Get(AuthToken token, int taskId)
    {
        var response = GetDataByUrl(HttpMethod.Get, _tasksControllerUrl + $"/{taskId}", token);
        TaskModel task = JsonConvert.DeserializeObject<TaskModel>(response);
        return task;
    }
    
    public List<TaskModel> GetTasksByDesk(AuthToken token, int deskId)
    {
        string responce = GetDataByUrl(HttpMethod.Get, _tasksControllerUrl + "?deskId=3", token);
        List<TaskModel> tasks = JsonConvert.DeserializeObject<List<TaskModel>>(responce);
        return tasks;
    }
    
    public HttpStatusCode Create(AuthToken token, TaskModel task)
    {
        string taskJson = JsonConvert.SerializeObject(task);
        var result = SendDataByUrl(HttpMethod.Post, _tasksControllerUrl, token, taskJson );
        return result;
    }
    
    public HttpStatusCode Update(AuthToken token, TaskModel task)
    {
        string taskJson = JsonConvert.SerializeObject(task);
        var result = SendDataByUrl(HttpMethod.Patch, _tasksControllerUrl + $"/{task.Id}", token, taskJson );
        return result;
    }
    
    public HttpStatusCode Delete(AuthToken token, int taskId)
    {
        var result = DeleteDataByUrl( _tasksControllerUrl + $"/{taskId}", token);
        return result;
    }
}
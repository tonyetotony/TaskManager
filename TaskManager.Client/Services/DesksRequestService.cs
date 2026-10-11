using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services;

public class DesksRequestService : CommonRequestService
{
    private string _desksControllerUrl = HOST + "desks";
    
    public List<DeskModel> GetDesksForCurrentUser(AuthToken token)
    {
        string responce = GetDataByUrl(HttpMethod.Get, _desksControllerUrl, token);
        List<DeskModel> desks = JsonConvert.DeserializeObject<List<DeskModel>>(responce);
        return desks;
    }
    
    public DeskModel Get(AuthToken token, int deskId)
    {
        var response = GetDataByUrl(HttpMethod.Get, _desksControllerUrl + $"/{deskId}", token);
        DeskModel desk = JsonConvert.DeserializeObject<DeskModel>(response);
        return desk;
    }
    
    public List<DeskModel> GetProjectDesks(AuthToken token, int projectId)
    {
        string responce = GetDataByUrl(HttpMethod.Get, _desksControllerUrl + $"/project?projectId={projectId}", token);
        List<DeskModel> desks = JsonConvert.DeserializeObject<List<DeskModel>>(responce);
        return desks ?? new List<DeskModel>();
    }
    
    public HttpStatusCode Create(AuthToken token, DeskModel desk)
    {
        string deskJson = JsonConvert.SerializeObject(desk);
        var result = SendDataByUrl(HttpMethod.Post, _desksControllerUrl, token, deskJson );
        return result;
    }
    
    public HttpStatusCode Update(AuthToken token, DeskModel desk)
    {
        string deskJson = JsonConvert.SerializeObject(desk);
        var result = SendDataByUrl(HttpMethod.Patch, _desksControllerUrl + $"/{desk.Id}", token, deskJson );
        return result;
    }
    
    public HttpStatusCode Delete(AuthToken token, int deskId)
    {
        var result = DeleteDataByUrl( _desksControllerUrl + $"/{deskId}", token);
        return result;
    }
}
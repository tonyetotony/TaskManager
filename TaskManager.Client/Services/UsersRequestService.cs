using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services;

public class UsersRequestService : CommonRequestService
{
    private string _usersControllerUrl = HOST + "users";
    

    public AuthToken GetToken(string username, string password)
    {
        string url = HOST + "account/token";
        string resultStr = GetDataByUrl(HttpMethod.Post, url, null,  username, password); 
        AuthToken token = JsonConvert.DeserializeObject<AuthToken>(resultStr);
        return token;
    }

    public HttpStatusCode CreateUser(AuthToken token, UserModel user)
    {
        string userJson = JsonConvert.SerializeObject(user);
        var result = SendDataByUrl(HttpMethod.Post, _usersControllerUrl, token, userJson );
        return result;
    }

    public List<UserModel> GetAllUsers(AuthToken token)
    {
        string rsponce = GetDataByUrl(HttpMethod.Get, _usersControllerUrl, token);
        List<UserModel> users = JsonConvert.DeserializeObject<List<UserModel>>(rsponce);
        return users;
    }
    
    public HttpStatusCode DeleteUser(AuthToken token, int userId)
    {
        var result = DeleteDataByUrl(_usersControllerUrl + $"/{userId}", token);
        return result;
    }
    
    public HttpStatusCode CreateUsers(AuthToken token, List<UserModel> users)
    {
        string userJson = JsonConvert.SerializeObject(users);
        var result = SendDataByUrl(HttpMethod.Post,_usersControllerUrl + "/all", token, userJson );
        return result;
    }
    
    public HttpStatusCode UpdateUser(AuthToken token, UserModel user)
    {
        string userJson = JsonConvert.SerializeObject(user);
        var result = SendDataByUrl(HttpMethod.Patch, _usersControllerUrl + $"/{user.Id}", token, userJson );
        return result;
    }
}
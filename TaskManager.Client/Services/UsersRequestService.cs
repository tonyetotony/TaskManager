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
    
    public UserModel GetCurrentUser(AuthToken token)
    {
        string responce = GetDataByUrl(HttpMethod.Get, HOST + "account/info", token);
        UserModel user = JsonConvert.DeserializeObject<UserModel>(responce);
        return user;
    }
    
    public UserModel GetUserById(AuthToken token, int userId)
    {
        try
        {
            if (userId <= 0)
                return null;
                
            string responce = GetDataByUrl(HttpMethod.Get, _usersControllerUrl + $"/{userId}", token);
            if (string.IsNullOrEmpty(responce))
                return null;
                
            UserModel user = JsonConvert.DeserializeObject<UserModel>(responce);
            return user;
        }
        catch
        {
            return null;
        }
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
    
    public int? GetProjectUserAdmin(AuthToken token, int userId)
    {
        var result = GetDataByUrl(HttpMethod.Get, _usersControllerUrl + $"/{userId}/admin", token);

        int adminId;

        bool parseResult = int.TryParse(result, out adminId);

        if (parseResult)
            return adminId;
        else
            return null;
    }
}
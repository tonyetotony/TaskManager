using System.Security.Claims;
using System.Text;
using TaskManager.Api.Models.Data;
using TaskManager.Common.Models;

namespace TaskManager.Api.Models.Services;

public class UserService : AbstractionService, ICommonService<UserModel>
{
    private readonly ApplicationContext _db;
    public UserService(ApplicationContext db)
    {
        _db = db;
    }

    public Tuple<string,string> GetUserLoginPassFromBasicAuth(HttpRequest request)
    {
        string userLogin = "";
        string userPass = "";
        string authHeader = request.Headers["Authorization"].ToString();
        if (authHeader != null && authHeader.StartsWith("Basic"))
        {
            string encodedUserNamePass = authHeader.Replace("Basic", "");
            var encoding = Encoding.GetEncoding("iso-8859-1");
            
            string[] namePassArray = encoding.GetString(Convert.FromBase64String(encodedUserNamePass)).Split(':');
            userLogin = namePassArray[0];
            userPass = namePassArray[1];
        }
        return new Tuple<string,string>(userLogin,userPass);
    }

    public User GetUser(string login, string pass)
    {
        var user = _db.Users.FirstOrDefault(u => u.Email == login && u.Password == pass);
        return user;
    }
    public User GetUser(string login)
    {
        var user = _db.Users.FirstOrDefault(u => u.Email == login);
        return user;
    }

    public ClaimsIdentity GetIdentity(string login, string pass)
    {
        User currentUser = GetUser(login, pass);
        if (currentUser != null)
        {
            currentUser.LastLoginDate = DateTime.Now;
            _db.Users.Update(currentUser);
            _db.SaveChanges();
            
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, currentUser.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, currentUser.Status.ToString())
            };
            ClaimsIdentity claimsIdentity = 
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, 
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
        return null;
        
    }

    public bool Create(UserModel model)
    {
        User newUser = new User(model.FirstName, model.LastName, model.Email, model.Password,
                model.Status, model.Phone, model.Photo);
        return DoAction(delegate()
        {
            _db.Users.Add(newUser);
            _db.SaveChanges();
        });
    }

    public bool Update(int id, UserModel model)
    {
        User userForUpdate = _db.Users.FirstOrDefault(u => u.Id == id);
        if (userForUpdate != null)
        {
            return DoAction(delegate()
            {
                if (!string.IsNullOrEmpty(model.FirstName))
                    userForUpdate.FirstName = model.FirstName;
                if (!string.IsNullOrEmpty(model.LastName))
                    userForUpdate.LastName = model.LastName;
                if (!string.IsNullOrEmpty(model.Email))
                    userForUpdate.Email = model.Email;
                if (!string.IsNullOrEmpty(model.Password))
                    userForUpdate.Password = model.Password;
                if (!string.IsNullOrEmpty(model.Phone))
                    userForUpdate.Phone = model.Phone;
                if (model.Photo != null && model.Photo.Length > 0)
                    userForUpdate.Photo = model.Photo;
                if (model.Status != default(UserStatus))
                    userForUpdate.Status = model.Status;
                
                _db.SaveChanges();
            });
        }
        return false;
    }

    public bool Delete(int id)
    {
        User user = _db.Users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            return DoAction(delegate()
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            });
        }

        return false;
    }

    public bool CreateUsers( List<UserModel> userModels)
    {
        return DoAction(delegate()
        {
            var newUsers = userModels.Select(u => new User(u));
            _db.Users.AddRange(newUsers);
            _db.SaveChanges();
        });
    }

    public UserModel Get(int id)
    {
        User userForUpdate = _db.Users.FirstOrDefault(u => u.Id == id);
        return userForUpdate?.ToDto();
       
    }

    public IEnumerable<UserModel> GetAllByIds(List<int> userIds)
    {
        foreach (int id in userIds)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == id).ToDto();
            yield return user;
        }
    }
}
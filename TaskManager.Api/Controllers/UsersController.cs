using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Data;
using TaskManager.Common.Models;


namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _db;
        public UsersController(ApplicationContext db) => _db = db;

        [HttpGet("test")]
        public IActionResult Test() => Ok("Hello");
        
        [HttpPost("create")]
        public IActionResult CreateUser([FromBody] UserModel userModel)
        {
            if (userModel != null)
            {
                User newUser = new User(userModel.FirstName, userModel.LastName, userModel.Email, userModel.Password,
                    userModel.Status, userModel.Phone, userModel.Photo);
                _db.Users.Add(newUser);
                _db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpPatch("update/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserModel userModel)
        {
            if (userModel != null)
            {
                User userForUpdate = _db.Users.FirstOrDefault(u => u.Id == id);
                if (userForUpdate != null)
                {
                    userForUpdate.FirstName = userModel.FirstName;
                    userForUpdate.LastName = userModel.LastName;
                    userForUpdate.Email = userModel.Email;
                    userForUpdate.Password = userModel.Password;
                    userForUpdate.Phone = userModel.Phone;
                    userForUpdate.Status = userModel.Status;
                    userForUpdate.Photo = userModel.Photo;
                    _db.Users.Update(userForUpdate);
                    _db.SaveChanges();
                    return Ok();
                }
                return NotFound();
            }
            return BadRequest();
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteUser(int id)
        {
            User user = _db.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<IEnumerable<UserModel>> GetUsers()
        {
            return await _db.Users.Select(u => u.ToDto()).ToListAsync();
        }

        [HttpPost("create/all")]
        public async Task<IActionResult> CreateUsers([FromBody] List<UserModel> userModels)
        {
            if (userModels != null && userModels.Count > 0)
            {
                var newUsers = userModels.Select(u => new User(u));
                _db.Users.AddRange(newUsers);
                await _db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }
    }
}

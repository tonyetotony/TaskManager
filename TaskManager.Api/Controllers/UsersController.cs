using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
    }
}

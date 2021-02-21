using BC = BCrypt.Net.BCrypt;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using smart_stock.Models;
using smart_stock.Services;
using System;

namespace smart_stock.Controllers
{
    [Route("api/user")]
    [ApiController]
    [EnableCors("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserProvider _userProvider;
        public UserController (IUserProvider userProvider)
        {
            _userProvider = userProvider;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {          
            Console.WriteLine("GET");  
            var test = await _userProvider.GetAllUsers();
            return test.ToList();
        }

        // GET: api/User/id
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userProvider.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User/id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            if (_userProvider.UserExists(id))
            {
                await _userProvider.UpdateUser(id, user);
            }  
            else 
            {
                return NotFound();
            }          

            return NoContent();
        }

        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            bool isUserTaken = await _userProvider.GetUserCredential(user.Credential.Username);
            if (isUserTaken)
            {
                List<Error> conflictJson = new List<Error>();
                conflictJson.Add(new Error() {ExceptionMessage = "This username already exists!", Tag = "User Controller", ApiArea = "User Controller"});
                return Conflict(conflictJson);
            }
            user.Credential.Password = BC.HashPassword(user.Credential.Password);
            var repoResult = await _userProvider.InsertUser(user);
            if (repoResult == null)
            {
                List<Error> errorJson = new List<Error>();
                errorJson.Add(new Error() {ExceptionMessage = "Failed to write user information", Tag = "User Provider", ApiArea = "User Controller"});
                return new JsonResult(errorJson);
            }
            return new JsonResult(repoResult);
        }

        // DELETE: api/User/id
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteUser(int id)
        {
            var user = await _userProvider.DeleteUser(id);
            if (!user)
            {
                return NotFound();
            }

            return user;
        }
    }
}
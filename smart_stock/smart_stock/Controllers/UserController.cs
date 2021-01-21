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
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("User")]
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
        public async Task<ActionResult<bool>> PostUser(User user)
        {
            return await _userProvider.InsertUser(user);
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
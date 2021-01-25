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
    [Route("api/login")]
    [ApiController]
    [EnableCors("login")]
    public class LoginController : ControllerBase
    {
        private readonly IUserProvider _userProvider;
        public LoginController (IUserProvider userProvider)
        {
            _userProvider = userProvider;
        }

        // GET: api/User/id
        [HttpGet("{username}")]
        public async Task<ActionResult<User>> GetUserLogin(string username)
        {
            if (username != null)
            {
                User user = await _userProvider.GetUserLogin(username);
                if (user != null)
                {
                    return Ok(user);
                }
                else {
                    return NotFound();
                }
            }
            return BadRequest();
        }
    }
}
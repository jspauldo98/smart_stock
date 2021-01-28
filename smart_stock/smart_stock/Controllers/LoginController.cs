using BC = BCrypt.Net.BCrypt;
using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using smart_stock.Models;
using smart_stock.Services;



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
        [HttpPost]
        public async Task<ActionResult<User>> GetUserLogin([FromBody] Credential credential)
        {
            if (ModelState.IsValid)
            {
                User user = await _userProvider.GetUserLogin(credential.Username, credential.Password);
                
                if (user != null)
                {
                    credential.Password = null;
                    user.Credential.Password = null;
                    return Ok(user);
                }
                else {
                    return NoContent();
                }
            }
            return BadRequest();
        }
    }
}
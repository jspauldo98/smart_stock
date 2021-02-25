using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using smart_stock.Models;
using smart_stock.Services;
using smart_stock.JwtManagement;



namespace smart_stock.Controllers
{
    [Route("api/login")]
    [ApiController]
    [EnableCors("login")]
    public class LoginController : ControllerBase
    {
        private readonly IUserProvider _userProvider;
        private readonly IJwtAuthManager _jwtAuthManager;
        public LoginController (IUserProvider userProvider, IJwtAuthManager jwtAuthManager)
        {
            _userProvider = userProvider;
            _jwtAuthManager = jwtAuthManager;
        }

        // GET: api/User/id
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<LoginResult>> GetUserLogin([FromBody] Credential credential)
        {
            if (ModelState.IsValid)
            {
                User user = await _userProvider.GetUserLogin(credential.Username, credential.Password);
                
                if (user != null)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, user.Credential.Username)
                    };
                    var jwtResult = _jwtAuthManager.GenerateTokens(user.Credential.Username, claims, DateTime.Now);
                    
                    LoginResult result = new LoginResult 
                    {
                        Username = user.Credential.Username,
                        AccessToken = jwtResult.AccessToken,
                        RefreshToken = jwtResult.RefreshToken.TokenString
                    };
                    return Ok(result);
                }
                else {
                    return NoContent();
                }
            }
            return BadRequest();
        }

        [HttpGet("user")]
        [Authorize]
        public ActionResult GetCurrentUser()
        {
            return Ok(new LoginResult
            {
                Username = User.Identity.Name
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public ActionResult UserLogout()
        {
            var username = User.Identity.Name;
            _jwtAuthManager.RemoveRefreshTokenByUserName(username);
            return Ok();
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<ActionResult<LoginResult>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var username = User.Identity.Name;
                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return Unauthorized();
                }

                var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                var jwtResult = _jwtAuthManager.Refresh(request.RefreshToken, accessToken, DateTime.Now);
                LoginResult result = new LoginResult {
                    Username = username,
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString
                };
                return Ok(result);
            }
            catch (SecurityTokenException exception)
            {
                return Unauthorized(exception.Message);
            }
        }
    }
}
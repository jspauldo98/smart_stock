using BC = BCrypt.Net.BCrypt;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using smart_stock.Models;
using smart_stock.Services;
using System;
using Microsoft.IdentityModel.Tokens;
using smart_stock.JwtManagement;

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

        [HttpGet("{userId}/{username}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> GetAllUserInformation([FromRoute] int userId, [FromRoute] string username)
        {
            var repoResult = await _userProvider.GetAllUserInformation(userId, username);
            if (repoResult == null)
            {
                List<Error> errorJson = new List<Error>();
                errorJson.Add(new Error() {ExceptionMessage = "Failed to get user information with header info: " + repoResult, Tag = "User Provider", ApiArea = "User Controller"});
                return new JsonResult(errorJson);
            }
            return new JsonResult(repoResult);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoreAPI.Models;
using CoreAPI.Services;
using CoreAPI.Utils;
using DB = CoreAPI.Utils.Database.MySql;

namespace CoreAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Create([FromForm]UserCreateModel model)
        {
            List<Dictionary<string, string>> lst = DB.Query($"SELECT * FROM user WHERE name = '{model.Username}'");
            if (lst.Count > 0)
                return BadRequest(new { title = "User already exists.", status = 400 });

            SHA384 sha = SHA384.Create();
            byte[] bSalt = sha.ComputeHash(Guid.NewGuid().ToByteArray());
            string salt = BitConverter.ToString(bSalt).Replace("-", "");
            string saltedPassword = DB.SaltedPassword(model.Password, salt);

            DB.Query($"INSERT INTO user (`name`,`hash`,`role`,`salt`)VALUES('{model.Username}','{saltedPassword}','{model.Role}','{salt}')");

            return CreatedAtAction("Create",null);
        }

        [AllowAnonymous]
        [HttpPost("auth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Authenticate([FromForm]AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }
    }
}
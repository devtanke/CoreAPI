using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using CoreAPI.Models;
using CoreAPI.Services;
using CoreAPI.Utils;
using DB = CoreAPI.Utils.Database.MySql;


namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("bye/{name?}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> Bye(string name)
        {
            return Ok(new { text= $"bye, {name}", numeric= 123 });
        }

    }
}
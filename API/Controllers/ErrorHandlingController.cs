using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    public class ErrorHandlingController : BaseApiController
    {
        private readonly ILogger<ErrorHandlingController> _logger;
        private readonly DataContext _context;
        public ErrorHandlingController(ILogger<ErrorHandlingController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        [HttpGet("auth")]
        [Authorize]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var user = _context.Users.Find(-1);
            if(user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("server-error")]
        public ActionResult<AppUser> GetServerError()
        {
            var user = _context.Users.Find(-1);
            user.ToString();
            return Ok(user);
        }

        [HttpGet("bad-request")]
        public ActionResult<AppUser> GetBadRequest()
        {
            var user = _context.Users.Find(-1);
            if(user == null)
            {
                return BadRequest();
            }
            return user;
        }
    }
}
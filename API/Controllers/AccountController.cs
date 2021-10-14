
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController: BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
        
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if(await IsUserExists(model.UserName)) return BadRequest("User name is taken.");
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = model.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
           await _context.SaveChangesAsync();
            return new UserDto 
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _context.Users.Include(x => x.Photos).SingleOrDefaultAsync(x=>x.UserName.ToLower().Equals(model.UserName.ToLower())); 
            if (user == null) return Unauthorized("Invalid user name.");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
            if(computedHash.Length > 0)
            {
                var hash = computedHash[0];
                var hash2 = user.PasswordHash[0];
                if(computedHash[0] != user.PasswordHash[0]) 
                return Unauthorized("Invalid password");
            }
            return new UserDto 
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                MainPhotoUrl = user.Photos.FirstOrDefault()?.Url
            };
        }
        public async Task<bool> IsUserExists(string userName)
        {
            bool res = await _context.Users.AnyAsync(x=>x.UserName.ToLower().Equals(userName.ToLower()));
            return res;
        }
    }
}
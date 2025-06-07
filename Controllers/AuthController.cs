using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrderDeliverySystem.Core;
using OrderDeliverySystem.Core.Dtos;
using OrderDeliverySystem.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OrderDeliverySystem.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly DataBaseContext _dataBaseContext;
        private readonly IConfiguration _configuration;
        public AuthController(DataBaseContext dataBaseContext, IConfiguration configuration)
        {
            _dataBaseContext = dataBaseContext;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userRequest)
        {
            using var hmac = new HMACSHA512();

            var user = new User
            {
                UserName = userRequest.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userRequest.Password)),
                Role = userRequest.Role,
                PasswordSalt = hmac.Key
            };
            _dataBaseContext.Users.Add(user);
            await _dataBaseContext.SaveChangesAsync();
            return Ok("Registered Successfully!");
        }

        [HttpPost("login")]
        public IActionResult Login(UserDto userRequest)
        {
            var user = _dataBaseContext.Users.FirstOrDefault(x => x.UserName == userRequest.UserName);
            if (user == null) { return Unauthorized("Invalid Username!"); }

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userRequest.Password));
            if (!computedHash.SequenceEqual(user.PasswordHash)) { return Unauthorized("Invalid Password!"); }
            Token token = CreateToken(user);
            return Ok(token);
        }
        private Token CreateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_configuration["TokenKey"] ?? "SuperSecretKey"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds);
            Token getToken = new Token
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            };
            return getToken;
        }

        public class Token
        {
            public string token { get; set; }
        }
    }
}

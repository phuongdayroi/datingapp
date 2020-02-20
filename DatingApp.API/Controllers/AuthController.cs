using DatingApp.API.Data.IRepository;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _auth;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository auth, IConfiguration configuration)
        {
            _auth = auth;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegister)
        {

            // validate request 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            userForRegister.Username = userForRegister.Username.ToLower();

            if (await _auth.UserExists(userForRegister.Username))
            {
                return BadRequest("User already existed");
            }

            var userToCreate = new User
            {
                Username = userForRegister.Username,
            };

            var createdUser = await _auth.Register(userToCreate, userForRegister.Password);

            return StatusCode(201);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Dtos.UserForLoginDto userForLogin)
        {
            try
            {
                var userFromRepo = await     _auth.Login(userForLogin.Username.ToLower(), userForLogin.Password);

                if (userForLogin == null) return Unauthorized();

                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = creds
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new
                {
                    token = tokenHandler.WriteToken(token)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Username or password is not correct!");
            }

        }
    }

}

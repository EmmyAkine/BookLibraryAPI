using BookLibraryAPI.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookLibraryAPI.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration config) {
            _userManager = userManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto) {
            var user = new IdentityUser { UserName = dto.Username, Email = dto.Email };
            var identityResult = await _userManager.CreateAsync(user, dto.Password);

            if (!identityResult.Succeeded) {
                return BadRequest(identityResult.Errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded) {
                // Roll back the user creation
                await _userManager.DeleteAsync(user);

                return StatusCode(500, new {
                    Message = "User registration failed while assigning role."
                });
            }

            return Ok("User registered.");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto) {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password)) {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim> {
               new(ClaimTypes.Name, user.UserName!)
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}

/*public class TestClass<TTest> where TTest : class {
    public void TestMethod(TTest param) {
        Console.WriteLine($"Type of T: {typeof(TTest)}");
        Console.WriteLine($"Value of param: {param}");
    }
}*/

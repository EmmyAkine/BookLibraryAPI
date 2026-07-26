using BookLibraryAPI.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookLibraryAPI.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase {
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(UserManager<IdentityUser> userManager) {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto) {
            var user = new IdentityUser { UserName = dto.Username, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User registered.");
        }

    }
}

/*public class TestClass<TTest> where TTest : class {
    public void TestMethod(TTest param) {
        Console.WriteLine($"Type of T: {typeof(TTest)}");
        Console.WriteLine($"Value of param: {param}");
    }
}*/

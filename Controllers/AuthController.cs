using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using User_Management.Constant;
using User_Management.Model;
using User_Management.ViewModel;

namespace User_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        //private readonly RoleManager<IdentityRole> _roleManager;
        public AuthController(UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            //_roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel model)
        {
            try
            {
                var isExist = await _userManager.FindByEmailAsync(model.Email);
                if (isExist == null)
                {
                    IdentityUser user = new();
                    user.Email = model.Email;
                    user.UserName = model.Username;
                    user.SecurityStamp = Guid.NewGuid().ToString();

                    //var roleExist = await _roleManager.RoleExistsAsync(signup.Role);
                    //if (roleExist)
                    //{
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        //await _userManager.AddToRoleAsync(user, signup.Role);
                        return StatusCode(StatusCodes.Status200OK,
                        new ResponseModel { status = "200", message = "user added successfully" });
                    }
                    else
                        return StatusCode(StatusCodes.Status404NotFound,
                        new ResponseModel { status = "Error", message = "failed to create user" });
                    //}
                    //else
                    //    return StatusCode(StatusCodes.Status404NotFound,
                    //   new Response { status = "Error", message = "role not found" });
                }
                else
                    return StatusCode(StatusCodes.Status302Found,
                        new ResponseModel { status = "Error", message = "user already exist" });
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { status = "Error", message = ex.Message });
            }
   
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var result = await _userManager.FindByEmailAsync(model.Email);
                if (result != null && await _userManager.CheckPasswordAsync(result, model.Password))
                {
                    TokenService tokenService = new TokenService();
                    var tokenResponse = await tokenService.GenerateTokens(result, _configuration, _context);
                    return StatusCode(StatusCodes.Status200OK,
                            new LoginResponseModel { status = "200", message = "Login Successfully", obj = tokenResponse });
                }
                else
                    return StatusCode(StatusCodes.Status302Found,
                       new ResponseModel { status = "Error", message = "user not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { status = "Error", message = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var storedToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken && !x.IsRevoked);
                if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
                    return Unauthorized(new { Message = "Invalid or expired refresh token" });

                var user = await _userManager.FindByIdAsync(storedToken.UserId);
                if (user == null) return Unauthorized();

                storedToken.IsRevoked = true;
                await _context.SaveChangesAsync();

                TokenService tokenService = new TokenService();
                var newTokens = await tokenService.GenerateTokens(user, _configuration, _context);
                return Ok(newTokens);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { status = "Error", message = ex.Message });
            }

        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            try
            {
                var storedToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken);
                if (storedToken != null)
                {
                    storedToken.IsRevoked = true;
                    await _context.SaveChangesAsync();
                    return Ok(new { Message = "Logged out successfully" });
                }
                else
                    return Unauthorized(new { Message = "Refresh token not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { status = "Error", message = ex.Message });
            }
        }
    }
}

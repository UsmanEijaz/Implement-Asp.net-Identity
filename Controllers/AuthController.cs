using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using User_Management.Model;

namespace User_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        //private readonly RoleManager<IdentityRole> _roleManager;
        public AuthController(UserManager<IdentityUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            //_roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel model)
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
                        new ResponseModel { status = "Error", message = "user added successfully" });
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var resut = await _userManager.FindByEmailAsync(model.Email);
            if(resut!=null && await _userManager.CheckPasswordAsync(resut,model.Password))
            {
                var authClaims = new List<Claim> 
                {
                    new Claim(ClaimTypes.Name, resut.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(2),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey,SecurityAlgorithms.HmacSha256)
                    );

                LoginTokenModel loginTokenModel = new ();
                loginTokenModel.token = new JwtSecurityTokenHandler().WriteToken(token);
                loginTokenModel.expiration = token.ValidTo;
                return StatusCode(StatusCodes.Status200OK,
                        new LoginResponseModel { status = "Error", message = "Login Successfully", obj = loginTokenModel });
            }
            else 
                return StatusCode(StatusCodes.Status302Found,
                   new ResponseModel { status = "Error", message = "user not found" });
        }
    }
}

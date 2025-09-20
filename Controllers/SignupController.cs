using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using User_Management.Model;

namespace User_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignupController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public SignupController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<ActionResult> RegisterUser([FromBody] Signup signup)
        {
            var isExist = await _userManager.FindByEmailAsync(signup.Email);
            if (isExist == null)
            {
                IdentityUser user = new();
                user.Email = signup.Email;
                user.UserName = signup.Username;
                user.SecurityStamp = Guid.NewGuid().ToString();

                var roleExist = await _roleManager.RoleExistsAsync(signup.Role);
                if (roleExist)
                {
                    var addUser = await _userManager.CreateAsync(user, signup.Password);
                    if (addUser.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, signup.Role);
                        return StatusCode(StatusCodes.Status200OK,
                        new Response { status = "Error", message = "user added successfully" });
                    }
                    else
                        return StatusCode(StatusCodes.Status404NotFound,
                        new Response { status = "Error", message = "failed to create user" });
                }
                else
                    return StatusCode(StatusCodes.Status404NotFound,
                   new Response { status = "Error", message = "role not found" });
            }
            else
                return StatusCode(StatusCodes.Status302Found,
                    new Response { status = "Error", message = "user already exist" });
        }
    }
}

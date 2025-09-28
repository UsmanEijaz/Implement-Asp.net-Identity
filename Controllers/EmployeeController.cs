using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User_Management.Model;
using User_Management.ViewModel;

namespace User_Management.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public EmployeeController(ApplicationDbContext context) 
        {
            _context = context; 
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUsers() 
        {
            var user = await _context.Users.FirstOrDefaultAsync();
            return StatusCode(StatusCodes.Status200OK, new { user = user });
        }
    }
}

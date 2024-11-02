using Microsoft.AspNetCore.Mvc;
using UserManagementApi.Data;
using UserManagementApi.Models;
using Microsoft.EntityFrameworkCore;


namespace UserManagementApi.Controllers
{
    [Route("api/v2/users")]
    [ApiController]
    public class UserV2Controller : ControllerBase
    {
        private readonly UserContext _context;

        public UserV2Controller(UserContext context)
        {
            _context = context;
        }

        // GET: api/v2/users

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
    }
}
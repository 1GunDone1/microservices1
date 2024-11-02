using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagementApi.Data;
using UserManagementApi.Models;
using Microsoft.AspNetCore.SignalR;
using UserManagementApi.Hubs;

namespace UserManagementApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<UserStatusHub> _hubContext;

        public AuthController(UserContext context, IConfiguration configuration, IHubContext<UserStatusHub> hubContext)
        {
            _context = context;
            _configuration = configuration;
            _hubContext = hubContext;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == loginUser.Username && u.Password == loginUser.Password);

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            // Обновление статуса пользователя
            user.IsOnline = true;
            user.LastLogin = DateTime.UtcNow;
            _context.SaveChanges();

            // Генерация JWT токена
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                }),
                Expires = DateTime.UtcNow.AddMinutes(5), // Токен действителен 5 минут
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Уведомление клиентов через SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveUserStatusChange", user.Id, true);

            return Ok(new { Token = tokenString });
        }
        // POST: api/auth/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized("User not authenticated");
            }

            var user = _context.Users.Find(int.Parse(userId));
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Обновление статуса пользователя
            user.IsOnline = false;
            user.LastLogin = null;
            _context.SaveChanges();

            // Уведомление клиентов через SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveUserStatusChange", user.Id, false);

            return Ok("Logged out successfully");
        }
    }
}
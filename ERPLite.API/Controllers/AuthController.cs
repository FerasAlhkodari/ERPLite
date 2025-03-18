using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERPLite.Core.Domain;
using ERPLite.Infrastructure.Data;
using ERPLite.Infrastructure.ServiceComponents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // In a real app, validate credentials with password hashing
            var user = await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // For demo purposes, we're using mock roles
            var roles = new List<string> { "Admin", "User" };

            // Generate token
            var token = _jwtService.GenerateToken(user.Id, user.Username, user.Email, roles);

            return Ok(new
            {
                token,
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    roles
                }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Check if user already exists
            if (await _context.Set<User>().AnyAsync(u => u.Username == request.Username || u.Email == request.Email))
            {
                return BadRequest(new { message = "Username or email already in use" });
            }

            // Create new user (modified to match your User entity)
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.Password, // In a real app, hash this password
                IsActive = true,
                PreferredLanguage = "en", // Default language
                CreatedAt = DateTime.UtcNow
            };

            await _context.Set<User>().AddAsync(user);
            await _context.SaveChangesAsync();

            // Return token for auto-login
            var roles = new List<string> { "User" };
            var token = _jwtService.GenerateToken(user.Id, user.Username, user.Email, roles);

            return Ok(new
            {
                token,
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    roles
                }
            });
        }
    }

    // Modified request models to match
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
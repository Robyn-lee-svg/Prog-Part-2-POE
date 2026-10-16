using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Models;
using RaceDay.Data;
using RaceDay.DTOs;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayContext _context;
            private readonly IPasswordHasher<User> _passwordHasher;
        public AuthController( RaceDayContext context,IPasswordHasher<User> passwordHasher) {
            _context = context;
            _passwordHasher = passwordHasher;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            string email = dto.Email.Trim().ToLowerInvariant();
            bool emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == email);
            if (!string.Equals(dto.Role, "Participant",
                StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message = "New accounts can only register as Participants."
                });
            }
            if (emailExists)
            {
                return Conflict(new
                {
                    message = " An account with this email already exists."
                });
            }

            var user = new User
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = email,
                Role = dto.Role,
                Phone = dto.Phone
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();


            var response = new UserResponseDto
            {
                UserID = user.UserID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role
            };
            return Created("/api/auth/login", response);

        }

        [HttpPost("login")]
        public async Task<IActionResult>Login(LoginDto dto)
        {
            string Email = dto.Email.Trim().ToLowerInvariant();
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == Email);
            if(user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if(verification == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message = "Invailed email or password."
                });
            }

            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("Role", user.Role);
            return Ok(new UserResponseDto
            {
                UserID = user.UserID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,

            });

        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new
            {
                message = "You have logged out successfully."
            });
        }









    }
}


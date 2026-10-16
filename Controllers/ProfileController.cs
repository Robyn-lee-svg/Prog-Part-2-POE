using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public ProfileController(RaceDayContext context)
        {
            _context = context;
        }

        
        
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserID == userId.Value);

            if (profile == null)
            {
                return NotFound("Profile not found.");
            }

            return Ok(profile);
        }

        
        
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(Profile updatedProfile)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserID == userId.Value);

            if (profile == null)
            {
                return NotFound("Profile not found. Create a profile first.");
            }

            profile.Address = updatedProfile.Address;
            profile.ProfilePictureURL = updatedProfile.ProfilePictureURL;

            await _context.SaveChangesAsync();

            return Ok(profile);
        }
    }
}

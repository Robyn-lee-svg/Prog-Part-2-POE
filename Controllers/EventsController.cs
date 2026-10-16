using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public EventsController(RaceDayContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .AsNoTracking()
                .Select(e => new
                {
                    e.EventID,
                    e.OrganiserID,
                    e.Name,
                    e.Description,
                    e.EventDate,
                    e.Location,
                    e.Distance,
                    e.EventType
                })
                .ToListAsync();

            return Ok(events);
        }

        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .AsNoTracking()
                .Where(e => e.EventID == id)
                .Select(e => new
                {
                    e.EventID,
                    e.OrganiserID,
                    e.Name,
                    e.Description,
                    e.EventDate,
                    e.Location,
                    e.Distance,
                    e.EventType
                })
                .FirstOrDefaultAsync();

            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            return Ok(eventItem);
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateEvent(Event eventItem)
        {
            var organiserId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (organiserId == null ||
                !string.Equals(role, "Organiser",
                    StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(new
                {
                    message = "Please log in as an organiser to create events."
                });
            }

            if (eventItem.Distance <= 0)
            {
                return BadRequest(new
                {
                    message = "Event distance must be greater than zero."
                });
            }

            if (!new[] { "Run", "Walk", "Cycle" }
                .Contains(eventItem.EventType, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message = "Event type must be Run, Walk, or Cycle."
                });
            }

            eventItem.EventID = 0;
            eventItem.OrganiserID = organiserId.Value;

            
            eventItem.Organiser = null;
            eventItem.Categories = new List<Category>();
            eventItem.Enrolments = new List<Enrolment>();

            
            eventItem.EventType = char.ToUpper(eventItem.EventType[0])
                + eventItem.EventType.Substring(1).ToLower();

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = eventItem.EventID },
                new
                {
                    eventItem.EventID,
                    eventItem.OrganiserID,
                    eventItem.Name,
                    eventItem.Description,
                    eventItem.EventDate,
                    eventItem.Location,
                    eventItem.Distance,
                    eventItem.EventType
                });
        }

        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEvent(
            int id, Event updatedEvent)
        {
            var organiserId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (organiserId == null ||
                !string.Equals(role, "Organiser",
                    StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(new
                {
                    message = "Please log in as an organiser to update events."
                });
            }

            var existingEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (existingEvent == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            if (existingEvent.OrganiserID != organiserId.Value)
            {
                return Forbid();
            }

            if (updatedEvent.Distance <= 0)
            {
                return BadRequest(new
                {
                    message = "Event distance must be greater than zero."
                });
            }

            if (!new[] { "Run", "Walk", "Cycle" }
                .Contains(updatedEvent.EventType,
                    StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message = "Event type must be Run, Walk, or Cycle."
                });
            }

            existingEvent.Name = updatedEvent.Name;
            existingEvent.Description = updatedEvent.Description;
            existingEvent.EventDate = updatedEvent.EventDate;
            existingEvent.Location = updatedEvent.Location;
            existingEvent.Distance = updatedEvent.Distance;
            existingEvent.EventType =
                char.ToUpper(updatedEvent.EventType[0])
                + updatedEvent.EventType.Substring(1).ToLower();

            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var organiserId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (organiserId == null ||
                !string.Equals(role, "Organiser",
                    StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(new
                {
                    message = "Please log in as an organiser to delete events."
                });
            }

            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            if (eventItem.OrganiserID != organiserId.Value)
            {
                return Forbid();
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
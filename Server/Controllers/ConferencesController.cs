using AutoMapper;
using ConfTool.Server.Hubs;
using ConfTool.Server.Models;
using ConfTool.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ConfTool.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ConferencesController : ControllerBase
    {
        private readonly ConferencesDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<ConferencesHub> _hubContext;

        public ConferencesController(ConferencesDbContext context
            , IMapper mapper
            , IHubContext<ConferencesHub> hubContext)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper??throw new ArgumentNullException(nameof(mapper));
            _hubContext=hubContext??throw new ArgumentNullException(nameof(hubContext));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConferenceOverview>>> GetConferences()
        {
            var confs = await _context.Conferences.ToListAsync();

            return Ok(_mapper.Map<IEnumerable<ConferenceOverview>>(confs));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConferenceDetails>> GetConference(Guid id)
        {
            var conference = await _context.Conferences.FindAsync(id);

            if (conference == null)
            {
                return NotFound();
            }

            return _mapper.Map<Shared.DTO.ConferenceDetails>(conference);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutConference(Guid id, ConferenceDetails conference)
        {
            if (id != conference.ID)
            {
                return BadRequest();
            }

            var conf = _mapper.Map<Conference>(conference);
            _context.Entry(conf).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConferenceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<ConferenceDetails>> PostConference([FromBody]ConferenceDetails conference)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var conf = _mapper.Map<Conference>(conference);

            var entry = _context.Conferences.Add(conf);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("NewConferenceAdded", entry.Entity.ID);

            return Ok(entry.Entity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConference(Guid id)
        {
            var conference = await _context.Conferences.FindAsync(id);
            if (conference == null)
            {
                return NotFound();
            }

            _context.Conferences.Remove(conference);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ConferenceExists(Guid id)
        {
            return _context.Conferences.Any(e => e.ID == id);
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using VGLog.Data;
using VGLog.Models;
using VGLog.Services;

namespace VGLog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideogameController : Controller
    {
        private readonly AppDbContext _context;
        private readonly VideogameService _videogameService;

        public VideogameController(AppDbContext context, VideogameService videogameService)
        {
            _context = context;
            _videogameService = videogameService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<Videogame>> GetAllVideogamesAsync()
        {
            var result = await _videogameService.GetAllAsync();
            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();

        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<Videogame>> GetByIdVideogame(int id)
        {

            var result = await _videogameService.GetByIdAsync(id);
            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpPost("CreateVideogame")]
        public async Task<ActionResult<Videogame>> CreateVideogame([FromBody] Videogame videogame)
        {
            var created = await _videogameService.CreateAsync(videogame);
            if (created != null)
            {
                return CreatedAtAction(
                    nameof(GetByIdVideogame),
                    new { id = created.Id },
                    created
                );
            }

            return BadRequest();
        }

        [HttpDelete("DeleteVideogame/{id}")]
        public async Task<ActionResult<Videogame>> DeleteVideogame(int id)
        {
            var deleted = await _videogameService.DeleteAsync(id);
            if (deleted != null)
            {
                return CreatedAtAction(
                    nameof(GetByIdVideogame),
                    new { id = deleted.Id },
                    deleted
                );
            }

            return BadRequest();
        
        }

        [HttpPut("EditVideogame")]
        public async Task<ActionResult<Videogame>> UpdateVideogame([FromBody] Videogame videogame)
        {
            var updated = await _videogameService.UpdateAsync(videogame);
            if (updated != null)
            {
                return CreatedAtAction(
                    nameof(GetByIdVideogame),
                    new { id = updated.Id },
                    updated
                );
            }

            return BadRequest();


        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserGame(int id, [FromBody] UserGame userGame)
        {
            var result = await _context.UserGames.FindAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            result.Id = userGame.Id;
            
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

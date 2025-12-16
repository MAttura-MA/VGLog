using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VGLog.Data;
using VGLog.Models;
using VGLog.Services;

namespace VGLog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : Controller
    {
        private readonly AppDbContext _context;
        private readonly GenreService _genreService;

        public GenreController(AppDbContext context, GenreService genreService)
        {
            _context = context;
            _genreService = genreService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllGenresAsync()
        {
            var result = await _genreService.GetAllAsync();
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<Genre>> GetByIdGenres(int id)
        {
            var result = await _genreService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("CreateGenre")]
        public async Task<ActionResult<Genre>> CreateGenre([FromBody] Genre genre)
        {

            if (genre != null)
            {
                var created = await _genreService.CreateAsync(genre);
                if (created != null)
                {
                    return CreatedAtAction(
                        nameof(GetByIdGenres),
                        new { id = created.Id },
                        created
                    );
                }
            }

            return BadRequest();
        }

        [HttpDelete("DeleteGenre/{id}")]
        public async Task<ActionResult<Genre>> DeleteGenre(int id)
        {

            var deleted = await _genreService.DeleteAsync(id);

            if (deleted != null)
            {
                return CreatedAtAction(
                    nameof(GetByIdGenres),
                    new { id = deleted.Id },
                    deleted
                );
            }

            return BadRequest();

        }

        [HttpPut("EditGenre")]
        public async Task<ActionResult<Genre>> UpdateGenre([FromBody] Genre genre)
        {
            var updated = await _genreService.UpdateAsync(genre);

            if (updated != null)
            {
                return CreatedAtAction(
                    nameof(GetByIdGenres),
                    new { id = updated.Id },
                    updated
                );
            }

            return BadRequest();


        }
    }
}

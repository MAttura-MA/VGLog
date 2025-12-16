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
    public class PlatformController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PlatformService _platformService;

        public PlatformController(AppDbContext context, PlatformService platformService)
        {
            _context = context;
            _platformService = platformService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllPlatformsAsync()
        {
            var result = await _platformService.GetAllAsync();
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<Platform>> GetByIdPlatform(int id)
        {
            var result = await _platformService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("CreatePlatform")]
        public async Task<ActionResult<Platform>> CreatePlatform([FromBody] Platform platform)
        {

            if (platform != null)
            {
                var created = await _platformService.CreateAsync(platform);
                if (created != null)
                {
                    return CreatedAtAction(
                        nameof(GetByIdPlatform),
                        new { id = created.Id },
                        created
                    );
                }
            }

            return BadRequest();
        }

        [HttpDelete("DeletePlatform/{id}")]
        public async Task<ActionResult<Platform>> DeletePlatform(int id)
        {

            var deleted = await _platformService.DeleteAsync(id);

            if (deleted != null)
            {
                return CreatedAtAction(
                    nameof(GetByIdPlatform),
                    new { id = deleted.Id },
                    deleted
                );
            }

            return BadRequest();

        }

        [HttpPut("EditPlatform")]
        public async Task<ActionResult<Platform>> UpdatePlatform([FromBody] Platform platform)
        {
            var updated = await _platformService.UpdateAsync(platform);

            if (updated != null)
            {
                return CreatedAtAction(
                    nameof(GetByIdPlatform),
                    new { id = updated.Id },
                    updated
                );
            }

            return BadRequest();


        }
    }
}


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
    public class SoftwareHouseController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SoftwareHouseService _sHService;

        public SoftwareHouseController(AppDbContext context, SoftwareHouseService softwareHouseService)
        {
            _context = context;
            _sHService = softwareHouseService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<SoftwareHouse>> GetAllSHousesAsync()
        {
            var result = await _sHService.GetAllAsync();
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<SoftwareHouse>> GetByIdSHouses(int id)
        {
            var result = await _sHService.GetByIdAsync(id);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpPost("CreateSHouse")]
        public async Task<ActionResult<SoftwareHouse>> CreateSHouses([FromBody] SoftwareHouse softwareHouse)
        {
            if (softwareHouse != null)
            {
                var created = await _sHService.CreateAsync(softwareHouse);

                if (created != null)
                {
                    return CreatedAtAction(
                        nameof(GetByIdSHouses),
                        new { id = created.Id },
                        created
                    );
                }

            }

            return BadRequest();
        }

        [HttpDelete("DeleteSHouse/{id}")]
        public async Task<ActionResult<SoftwareHouse>> DeleteSHouses(int id)
        {
            var deleted = await _sHService.DeleteAsync(id);
            if (deleted != null)
            {
                return CreatedAtAction(
                    nameof(GetByIdSHouses),
                    new { id = deleted.Id },
                    deleted
                );
            }

            return BadRequest();

        }

        [HttpPut("EditShouse")]
        public async Task<ActionResult<SoftwareHouse>> UpdateSHouses([FromBody] SoftwareHouse softwareHouse)
        {
            var updated = await _sHService.UpdateAsync(softwareHouse);

            if (updated != null)
            {
                return CreatedAtAction(
                    nameof(GetByIdSHouses),
                    new { id = updated.Id },
                    updated
                );
            }

            return BadRequest();


        }
    }
}

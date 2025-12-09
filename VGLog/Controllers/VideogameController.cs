using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VGLog.Data;
using VGLog.Models;

namespace VGLog.Controllers
{
    public class VideogameController : Controller
    {
        private readonly AppDbContext _context;

        public VideogameController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Videogame
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Videogames.Include(v => v.SoftwareHouse);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Videogame/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videogame = await _context.Videogames
                .Include(v => v.SoftwareHouse)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (videogame == null)
            {
                return NotFound();
            }

            return View(videogame);
        }

        // GET: Videogame/Create
        public IActionResult Create()
        {
            ViewData["SoftwareHouseId"] = new SelectList(_context.SoftwareHouses, "Id", "Id");
            return View();
        }

        // POST: Videogame/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ReleaseYear,Image,Status,AddedAt,CompletedAt,PersonalRating,Notes,SoftwareHouseId")] Videogame videogame)
        {
            if (ModelState.IsValid)
            {
                _context.Add(videogame);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SoftwareHouseId"] = new SelectList(_context.SoftwareHouses, "Id", "Id", videogame.SoftwareHouseId);
            return View(videogame);
        }

        // GET: Videogame/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videogame = await _context.Videogames.FindAsync(id);
            if (videogame == null)
            {
                return NotFound();
            }
            ViewData["SoftwareHouseId"] = new SelectList(_context.SoftwareHouses, "Id", "Id", videogame.SoftwareHouseId);
            return View(videogame);
        }

        // POST: Videogame/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ReleaseYear,Image,Status,AddedAt,CompletedAt,PersonalRating,Notes,SoftwareHouseId")] Videogame videogame)
        {
            if (id != videogame.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(videogame);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideogameExists(videogame.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SoftwareHouseId"] = new SelectList(_context.SoftwareHouses, "Id", "Id", videogame.SoftwareHouseId);
            return View(videogame);
        }

        // GET: Videogame/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videogame = await _context.Videogames
                .Include(v => v.SoftwareHouse)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (videogame == null)
            {
                return NotFound();
            }

            return View(videogame);
        }

        // POST: Videogame/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var videogame = await _context.Videogames.FindAsync(id);
            if (videogame != null)
            {
                _context.Videogames.Remove(videogame);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VideogameExists(int id)
        {
            return _context.Videogames.Any(e => e.Id == id);
        }
    }
}

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
    public class SoftwareHouseController : Controller
    {
        private readonly AppDbContext _context;

        public SoftwareHouseController(AppDbContext context)
        {
            _context = context;
        }

        // GET: SoftwareHouse
        public async Task<IActionResult> Index()
        {
            return View(await _context.SoftwareHouses.ToListAsync());
        }

        // GET: SoftwareHouse/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var softwareHouse = await _context.SoftwareHouses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (softwareHouse == null)
            {
                return NotFound();
            }

            return View(softwareHouse);
        }

        // GET: SoftwareHouse/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SoftwareHouse/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Country,FoundedYear")] SoftwareHouse softwareHouse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(softwareHouse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(softwareHouse);
        }

        // GET: SoftwareHouse/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var softwareHouse = await _context.SoftwareHouses.FindAsync(id);
            if (softwareHouse == null)
            {
                return NotFound();
            }
            return View(softwareHouse);
        }

        // POST: SoftwareHouse/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Country,FoundedYear")] SoftwareHouse softwareHouse)
        {
            if (id != softwareHouse.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(softwareHouse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SoftwareHouseExists(softwareHouse.Id))
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
            return View(softwareHouse);
        }

        // GET: SoftwareHouse/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var softwareHouse = await _context.SoftwareHouses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (softwareHouse == null)
            {
                return NotFound();
            }

            return View(softwareHouse);
        }

        // POST: SoftwareHouse/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var softwareHouse = await _context.SoftwareHouses.FindAsync(id);
            if (softwareHouse != null)
            {
                _context.SoftwareHouses.Remove(softwareHouse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SoftwareHouseExists(int id)
        {
            return _context.SoftwareHouses.Any(e => e.Id == id);
        }
    }
}

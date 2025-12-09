using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VGLog.Data;
using VGLog.Models;

namespace VGLog.Services
{
    public class VideogameService
    {
        private readonly AppDbContext _context;

        public VideogameService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Videogame>> GetAllAsync()
        {
            return await _context.Videogames
                .ToListAsync();
        }

        public async Task<Videogame> GetByIdAsync(int Id)
        {

            return await _context.Videogames.FindAsync(Id);

        }

        public async Task CreateAsync(Videogame videogame)
        {
            _context.Videogames.Add(videogame);
            await _context.SaveChangesAsync();

        }

        public async Task DeleteAsync(int Id)
        {
            var entity = await _context.Videogames.FindAsync(Id);

            if (entity != null)
            {
                _context.Videogames.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Videogame videogame)
        {
            var entity = await _context.Videogames.FindAsync(videogame.Id); 

            if (entity != null)
            {
                _context.Videogames.Update(videogame);
                await _context.SaveChangesAsync();
            }
        }
    }
}

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using VGLog.Data;
using VGLog.Models;

namespace VGLog.Services
{
    public class SoftwareHouseService
    {
        private readonly AppDbContext _context;

        public SoftwareHouseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SoftwareHouse>> GetAllAsync()
        {
            return await _context.SoftwareHouses
                .ToListAsync();
        }

        public async Task<SoftwareHouse> GetByIdAsync(int Id)
        {
            
            return await _context.SoftwareHouses.FindAsync(Id);
                
        }

        public async Task CreateAsync(SoftwareHouse softwareHouse)
        {
            _context.SoftwareHouses.Add(softwareHouse);
            await _context.SaveChangesAsync();

        }

        public async Task DeleteAsync(int Id)
        {
            var entity = await _context.SoftwareHouses.FindAsync(Id);

            if (entity != null)
            {
                _context.SoftwareHouses.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(SoftwareHouse softwareHouses)
        {
            var entity = await _context.Videogames.FindAsync(softwareHouses.Id);

            if (entity != null)
            {
                _context.SoftwareHouses.Update(softwareHouses);
                await _context.SaveChangesAsync();
            }
        }
    }
}

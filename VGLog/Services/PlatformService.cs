using Microsoft.EntityFrameworkCore;
using VGLog.Data;
using VGLog.Models;

namespace VGLog.Services
{
    public class PlatformService
    {
        private readonly AppDbContext _context;

        public PlatformService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Platform>> GetAllAsync()
        {
            return await _context.Platforms
                .ToListAsync();
        }

        public async Task<Platform> GetByIdAsync(int Id)
        {

            return await _context.Platforms.FindAsync(Id);

        }

        public async Task CreateAsync(Platform platform)
        {
            _context.Platforms.Add(platform);
            await _context.SaveChangesAsync();

        }

        public async Task DeleteAsync(int Id)
        {
            var entity = await _context.Platforms.FindAsync(Id);

            if (entity != null)
            {
                _context.Platforms.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Platform platform)
        {
            var entity = await _context.Platforms.FindAsync(platform.Id);

            if (entity != null)
            {
                _context.Platforms.Update(platform);
                await _context.SaveChangesAsync();
            }
        }
    }
}

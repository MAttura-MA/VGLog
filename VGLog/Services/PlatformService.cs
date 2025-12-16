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

        public async Task<Platform?> GetByIdAsync(int Id)
        {

            return await _context.Platforms.FindAsync(Id);

        }

        public async Task<Platform?> CreateAsync(Platform platform)
        {
            if (platform != null)
            {

            _context.Platforms.Add(platform);
            await _context.SaveChangesAsync();

            return platform;

            }

            return null;

        }

        public async Task<Platform?> DeleteAsync(int? Id)
        {
            if (Id != null)
            {
                var entity = await _context.Platforms.FindAsync(Id);

                if (entity != null)
                {
                    _context.Platforms.Remove(entity);
                    await _context.SaveChangesAsync();

                    return entity;
                }

            }

            return null;

        }

        public async Task<Platform?> UpdateAsync(Platform platform)
        {
            if ( platform != null)
            {
                var entity = await _context.Platforms.FindAsync(platform.Id);

                if (entity != null)
                {
                    _context.Platforms.Update(platform);
                    await _context.SaveChangesAsync();

                    return entity;
                }
                
            }

            return null;
        }
    }
}

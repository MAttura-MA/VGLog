using Microsoft.EntityFrameworkCore;
using VGLog.Data;
using VGLog.Models;

namespace VGLog.Services
{
    public class GenreService
    {
        private readonly AppDbContext _context;

        public GenreService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Genre>> GetAllAsync()
        {
            return await _context.Genres
                .ToListAsync();
        }

        public async Task<Genre?> GetByIdAsync(int Id)
        {

            return await _context.Genres.FindAsync(Id);

        }

        public async Task<Genre?> CreateAsync(Genre genre)
        {
            if (genre != null)
            {
                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();
                return genre;
            }

            return null;

        }

        public async Task<Genre?> DeleteAsync(int? Id)
        {
            if (Id != null)
            {
                var entity = await _context.Genres.FindAsync(Id);

                if (entity != null)
                {
                    _context.Genres.Remove(entity);
                    await _context.SaveChangesAsync();

                    return entity;
                }

            }

            return null;

        }

        public async Task<Genre?> UpdateAsync(Genre genre)
        {
            if (genre != null)
            {
                var entity = await _context.Genres.FindAsync(genre.Id);
                if (entity != null)
                {
                    _context.Genres.Update(genre);
                    await _context.SaveChangesAsync();

                    return entity;
                }

            }

            return null;

        }
    }
}

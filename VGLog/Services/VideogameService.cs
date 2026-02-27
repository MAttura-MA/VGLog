using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VGLog.Data;
using VGLog.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using VGLog.Services.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace VGLog.Services
{
    public class VideogameService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VideogameService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<Videogame>> GetAllAsync()
        {
            return await _context.Videogames
                .ToListAsync();
        }

        public async Task<Videogame?> GetByIdAsync(int Id)
        {

            return await _context.Videogames.FindAsync(Id);

        }

        public async Task<Videogame?> CreateAsync(Videogame videogame)
        {
            if (videogame != null)
            {
                _context.Videogames.Add(videogame);
                await _context.SaveChangesAsync();

                return videogame;
            }

            return null;

        }

        public async Task<Videogame?> DeleteAsync(int Id)
        {
            var entity = await _context.Videogames.FindAsync(Id);

            if (entity != null)
            {
                _context.Videogames.Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }

            return null;
        }

        public async Task<Videogame?> UpdateAsync(Videogame videogame)
        {
            var entity = await _context.Videogames.FindAsync(videogame.Id); 

            if (entity != null)
            {
                _context.Videogames.Update(videogame);
                await _context.SaveChangesAsync();
                return entity;
            }

            return null;
        }

        

        public async Task<List<Videogame>> SearchGamesAsync(string query)
        {
            return await _context.Videogames
                .Where(g => g.Title.Contains(query))
                .OrderBy(g => g.Title)
                .Take(10)
                .ToListAsync();
        }

        

    }
}

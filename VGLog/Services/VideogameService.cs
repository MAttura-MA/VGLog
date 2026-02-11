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

        public async Task<UserGame> AddGameToUserAsync(int videogameId, int? personalRating, string? notes, ClaimsPrincipal user, GameStatus PlayedOrNot, int? HoursPlayed)
        {
            var userId = _userManager.GetUserId(user);

            var gameExists = await _context.Videogames
                .FirstOrDefaultAsync(g => g.Id == videogameId);

            if (gameExists != null)
            {
                var alreadyAdded = await _context.UserGames
                    .AnyAsync(ug => ug.UserId == userId && ug.VideogameId == videogameId);

                if (alreadyAdded)
                    throw new Exception("This game is already in your profile");

                var userGame = new UserGame
                {
                    UserId = userId,
                    VideogameId = videogameId,
                    PersonalRating = personalRating,
                    Completed = personalRating.HasValue,
                    CompletedAt = personalRating.HasValue ? DateTime.Now : null,
                    GameStatus = PlayedOrNot,
                    HoursPlayed = HoursPlayed,
                    Notes = notes
                };

                _context.UserGames.Add(userGame);
                await _context.SaveChangesAsync();

                return userGame;
            }

            throw new Exception("Game not found");

        }

        public async Task<List<UserGame>> GetUserGamesAsync(ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);

            return await _context.UserGames
                .Include(ug => ug.Videogame)
                .Where(ug =>  ug.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Videogame>> SearchGamesAsync(string query)
        {
            return await _context.Videogames
                .Where(g => g.Title.Contains(query))
                .OrderBy(g => g.Title)
                .Take(10)
                .ToListAsync();
        }

        public async Task EditUserGame(int userGameId, int? hours)
        {
            var result = await _context.UserGames
                .FirstOrDefaultAsync(u => u.Id == userGameId);

            if (result == null)
                return;

            result.HoursPlayed = hours;

            await _context.SaveChangesAsync();
        }


    }
}

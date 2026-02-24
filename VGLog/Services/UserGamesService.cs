using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using VGLog.Data;
using VGLog.Models;
using VGLog.Services.DTOs;

namespace VGLog.Services
{
    public class UserGamesService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserGamesService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<GetUserGamesDto> GetUserGamesWithCounterAsync(string paramUserId)
        {
            var userGames = await _context.UserGames
                .Include(ug => ug.Videogame)
                .Where(ug => ug.UserId == paramUserId)
                .ToListAsync();

            var dto = new GetUserGamesDto
            {
                Games = userGames ?? new List<UserGame>(),
                Total = userGames?.Count ?? 0,
                Completed = userGames?.Count(g => g.GameStatus == GameStatus.Completed) ?? 0,
                Playing = userGames?.Count(g => g.GameStatus == GameStatus.Playing) ?? 0,
                ToPlay = userGames?.Count(g => g.GameStatus == GameStatus.Toplay) ?? 0,
                Dropped = userGames?.Count(g => g.GameStatus == GameStatus.Dropped) ?? 0,
                TotalHours = userGames?.Sum(g => g.HoursPlayed) ?? 0
            };

            return dto;
        }

        public async Task EditUserGameAsync(UpdateUserGameDto dto)
        {
            var userGame = await _context.UserGames
                .FirstOrDefaultAsync(u => u.Id == dto.Id);

            if (userGame == null)
                return;

            userGame.HoursPlayed = dto.HoursPlayed;
            userGame.Notes = dto.Notes;
            userGame.PersonalRating = dto.PersonalRating;

            await _context.SaveChangesAsync();
        }

        public async Task EditUserGameStatusAsync(int userGameId, GameStatus newStatus)
        {
            var userGame = await _context.UserGames
                .FirstOrDefaultAsync(u => u.Id == userGameId);

            if (userGame == null) return;

            userGame.GameStatus = newStatus;

            await _context.SaveChangesAsync();
        }


        public async Task<UserGame?> DeleteUserGameAsync(int Id)
        {
            var entity = await _context.UserGames.FindAsync(Id);

            if (entity != null)
            {
                _context.UserGames.Remove(entity);
                await _context.SaveChangesAsync();
                return entity;
            }

            await _context.SaveChangesAsync();

            return null;

        }

        public async Task<List<UserGame>> LastGamesAdded(string paramUserId)
        {
            var result = await _context.UserGames.Where(g => g.UserId == paramUserId)
                        .OrderByDescending(e => e.TimeStampAdded)
                        .Take(5)
                        .Include(g => g.Videogame)
                        .ToListAsync();

            return result;
        }
    }
}

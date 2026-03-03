using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Security.Claims;
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
                    CompletedAt = PlayedOrNot == GameStatus.Completed ? DateTime.Now : null,
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

        public async Task<double> GetAvgRating(string paramUserId)
        {
            var result = await _context.UserGames
                .Where(x => x.UserId == paramUserId && x.PersonalRating != null)
                .AverageAsync(x => x.PersonalRating.Value);

            return Math.Round(result, 1);
        }

        public async Task<UserGame?> GetMostPlayedGame(string paramUserId)
        {
            var result = await _context.UserGames
                .Where(x => x.UserId == paramUserId)
                .OrderByDescending(x => x.HoursPlayed)
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<UserGame?> GetRecentlyCompleteGame(string paramUserId)
        {
            var result = await _context.UserGames
                .Where(x => x.UserId == paramUserId)
                .OrderByDescending(x => x.CompletedAt)
                .FirstOrDefaultAsync();

            return result;
        }

        public double GetCompletionRate(int completed, int total, string paramUserId)
        {
            // Evita divisione per zero
            if (total == 0)
                return 0;

            // Calcola percentuale e arrotonda a 1 decimale
            return Math.Round((double)completed / total * 100, 1);
        }
    }

}

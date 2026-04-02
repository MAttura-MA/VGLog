using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VGLog.Data;
using VGLog.Models;
using VGLog.Models.Enums;
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

        public async Task<UserGame> AddGameToUserAsync(int videogameId, int? personalRating, string? notes, string paramUserId, GameStatusEnum PlayedOrNot, int? HoursPlayed)
        {
            var gameExists = await _context.Videogames
                .FirstOrDefaultAsync(g => g.Id == videogameId);

            if (gameExists != null)
            {

                var userGame = new UserGame
                {
                    UserId = paramUserId,
                    VideogameId = videogameId,
                    PersonalRating = personalRating,
                    CompletedAt = PlayedOrNot == GameStatusEnum.Completed ? DateTime.Now : null,
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

        public async Task<GetUserGamesDto> GetUserGamesByIdAsync(string paramUserId)
        {
            var userGames = await _context.UserGames
                .Include(ug => ug.Videogame)
                .Where(ug => ug.UserId == paramUserId)
                .ToListAsync();

            if (!userGames.Any())
                return new GetUserGamesDto();

            //counter semplici
            var completedGames = userGames.Count(x => x.GameStatus == GameStatusEnum.Completed);
            var playingGames = userGames.Count(x => x.GameStatus == GameStatusEnum.Playing);
            var toPlayGames = userGames.Count(x => x.GameStatus == GameStatusEnum.Toplay);
            var droppedGames = userGames.Count(x => x.GameStatus == GameStatusEnum.Dropped);
            var totalHours = userGames.Sum(x => x.HoursPlayed);

            var avgRating = userGames
                .Where(x => x.PersonalRating.HasValue)
                .Select(x => x.PersonalRating!.Value)
                .DefaultIfEmpty(0)
                .Average();

            var mostPlayedGame = userGames
                .OrderByDescending(x => x.HoursPlayed)
                .FirstOrDefault();

            var mostRecentlyCompletedGame = userGames
                .Where(x => x.GameStatus == GameStatusEnum.Completed && x.CompletedAt.HasValue)
                .OrderByDescending(x => x.CompletedAt)
                .FirstOrDefault();

            return new GetUserGamesDto
            {
                Games = userGames,
                Total = userGames.Count,
                Completed = completedGames,
                Playing = playingGames,
                ToPlay = toPlayGames,
                Dropped = droppedGames,
                TotalHours = totalHours,
                CompletionRate = userGames.Count > 0
                    ? Math.Round((double)completedGames / userGames.Count * 100, 1)
                    : 0,
                AvgRating = Math.Round(avgRating, 1),
                MostPlayedGame = mostPlayedGame,
                MostRecentlyCompletedGame = mostRecentlyCompletedGame
            };
        }

        public async Task EditUserGameAsync(UpdateUserGameDto dto)
        {
            var userGame = await _context.UserGames
                .FirstOrDefaultAsync(u => u.Id == dto.Id);

            if (userGame == null)
                throw new KeyNotFoundException($"Videogame not found."); ;

            userGame.HoursPlayed = dto.HoursPlayed;
            userGame.Notes = dto.Notes;
            userGame.PersonalRating = dto.PersonalRating;
            userGame.GameStatus = dto.GameStatus;
            userGame.CompletedAt = dto.GameStatus == GameStatusEnum.Completed ? (userGame.CompletedAt ?? DateTime.Now) : null;


            await _context.SaveChangesAsync();
        }

        public async Task EditUserGameStatusAsync(int userGameId, GameStatusEnum newStatus)
        {
            var userGame = await _context.UserGames
                .FirstOrDefaultAsync(u => u.Id == userGameId);

            if (userGame == null) return;

            userGame.GameStatus = newStatus;
            userGame.CompletedAt = newStatus == GameStatusEnum.Completed ? (userGame.CompletedAt ?? DateTime.Now) : null;

            await _context.SaveChangesAsync();
        }

        public async Task<UserGame?> DeleteUserGameAsync(int GameId, string paramUserId)
        {
            var entity = await _context.UserGames
                .Where(u => u.UserId == paramUserId && u.Id == GameId)
                .FirstOrDefaultAsync();

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

        public string GetDisplayName (ClaimsPrincipal user, string paramUserId)
        {
            var result = user.Claims
                .FirstOrDefault(c => c.Type == "DisplayName")?.Value ?? user.Identity.Name;

            return result ?? string.Empty;
        }
    }

}

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

        public async Task<GetUserGamesDto> GetUserGamesWithCounterAsync(string paramUserId)
        {
            var loggedInUserId = _userManager.GetUserId(user);

            var userGames = await _context.UserGames
                .Include(ug => ug.Videogame)
                .Where(ug =>  ug.UserId == paramUserId)
                .ToListAsync();

            var dto = new GetUserGamesDto
            {
                Games = userGames ?? new List<UserGame>(),
                Total = userGames?.Count ?? 0,
                Completed = userGames?.Count(g => g.GameStatus == GameStatus.Completed) ?? 0,
                Playing = userGames?.Count(g => g.GameStatus == GameStatus.Playing) ?? 0,
                ToPlay = userGames?.Count(g => g.GameStatus == GameStatus.Toplay) ?? 0,
                TotalHours = userGames?.Sum(g => g.HoursPlayed) ?? 0
            };

            return dto;
        }

 


        public async Task<List<Videogame>> SearchGamesAsync(string query)
        {
            return await _context.Videogames
                .Where(g => g.Title.Contains(query))
                .OrderBy(g => g.Title)
                .Take(10)
                .ToListAsync();
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

    }
}

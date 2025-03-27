using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using MinesGame.Models;
using MinesGame.Repository;
using MinesGame.ViewModels;

namespace MinesGame.Service;

public class PlayerService : IPlayerService
{

    public readonly IPlayerRepository _playerRepository;
    public PlayerService(IPlayerRepository _playerRepo)
    {
        _playerRepository = _playerRepo;
    }

    public async Task<bool> RegisterPlayerAsync(PlayerViewModel player)
    {
        bool isExisitingPlayer = await _playerRepository.IsExisitingPlayer(player.Username, player.Email);
        if (!isExisitingPlayer)
        {
            var newPlayer = new Player
            {
                Username = player.Username,
                Email = player.Email,
                Password = player.Password
            };
            await _playerRepository.AddPlayerAsync(newPlayer);
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> LoginPlayerAsync(HttpContext httpContext, string username, string password)
    {
        var existingPlayer = await _playerRepository.GetPlayerAsync(username);

        if (existingPlayer != null && existingPlayer.Password == password)
        {
            await AssignPlayerCookie(httpContext, existingPlayer.Username, existingPlayer.Email, "Player");
            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task AssignPlayerCookie(HttpContext httpContext, string username, string email, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        string cookie = CookieAuthenticationDefaults.AuthenticationScheme;
        var claimIdentity = new ClaimsIdentity(claims, cookie);
        var authProperties = new AuthenticationProperties { IsPersistent = true };

        await httpContext.SignInAsync
        (
            cookie,
            new ClaimsPrincipal(claimIdentity),
            authProperties
        );
        return;
    }

    public async Task<bool> IsSqlServerAvailableAsync()
    {
        return await _playerRepository.IsSqlServerRunning();
    }
}
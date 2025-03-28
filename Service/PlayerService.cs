using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
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

    public async Task<bool> RegisterPlayerAsync(PlayerViewModel playerVm)
    {
        bool isExisitingPlayer = await _playerRepository.CheckUsernameOrEmailExists(playerVm.Username, playerVm.Email);
        if (!isExisitingPlayer)
        {
            var newPlayer = new Player
            {
                Username = playerVm.Username,
                Email = playerVm.Email,
                Password = playerVm.Password
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
        string email = username;
        var existingPlayer = await _playerRepository.GetPlayerAsync(username, email);

        if (existingPlayer != null && existingPlayer.Password == password)
        {
            await AssignPlayerCookie(httpContext, existingPlayer.PlayerId, existingPlayer.Email, "Player");
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<PlayerViewModel> CreateDummyPlayer(HttpContext httpContext)
    {
        int playerId = GetCurrentPlayerId(httpContext);
        if (playerId == -1)
        {
            var dummyPlayer = new PlayerViewModel
            {
                Username = "",
                Email = "",
                Password = ""
            };
            return dummyPlayer;
        }
        else
        {
            var getDummyplayer = await _playerRepository.GetDummyPlayer(playerId);
            return getDummyplayer;
        }
    }

    public async Task<WalletDisplayViewModel> CreateDummyPlayerWallet(HttpContext httpContext)
    {
        int playerId = GetCurrentPlayerId(httpContext);
        if (playerId == -1)
        {
            var dummyWallet = new WalletDisplayViewModel{};
            return dummyWallet;
        }
        else
        {
            var getdummyWallet = await _playerRepository.GetDummyWallet(playerId);
            return getdummyWallet;
        }
    }


    public async Task<bool> EditPlayerAsync(HttpContext httpContext, PlayerViewModel playerVm)
    {
        int playerId = GetCurrentPlayerId(httpContext);
        bool isExistingUsernameOrEmail = await _playerRepository.CheckUsernameOrEmailExists(playerVm.Username, playerVm.Email);
        bool arePlayerFieldsValid = VerifyPlayerFields(playerVm);
        if (playerId == -1 || isExistingUsernameOrEmail == true || arePlayerFieldsValid == false)
        {
            return false;
        }
        else
        {
            await _playerRepository.UpdatePlayerDetailsAsync(playerId, playerVm);
            return true;
        }
    }

    public async Task<object> AddMoneyToWallet(HttpContext httpContext, string amountToAdd)
    {
        int playerId = GetCurrentPlayerId(httpContext);
        if (playerId == -1)
        {
            return new {success = false, message = "Player do not exists."};
        }
        else
        {
            decimal amountToDecimal = Convert.ToDecimal(amountToAdd);
            decimal playerBalance = await _playerRepository.GetPlayerBalanceDB(playerId);
            if(amountToDecimal < 100)
            {
                return new {success = false, message = "Cannot deposit less than 100rs."};
            }
            else if(playerBalance > 10000)
            {
                return new {success = false, message = "Wallet limit exceeds (10000rs)."};
            }
            else
            {
                await _playerRepository.AddMoneyToWalletDB(playerId, amountToDecimal);
                return new {success = true, message = "Money Deposited Successfully."};
            }
        }
    }

    public async Task<object> RemoveMoneyFromWallet(HttpContext httpContext, string amountToRemove)
    {
        int playerId = GetCurrentPlayerId(httpContext);
        if (playerId == -1)
        {
            return new {success = false, message = "Player do not exists."};
        }
        else
        {
            decimal amountToDecimal = Convert.ToDecimal(amountToRemove);
            decimal playerBalance = await _playerRepository.GetPlayerBalanceDB(playerId);
            if(amountToDecimal < 100)
            {
                return new {success = false, message = "Money Withdraw Error."};
            }
            else if(amountToDecimal > playerBalance)
            {
                return new {success = false, message = "Withdraw amount cannot exceed wallet balance."};
            }
            else
            {
                await _playerRepository.RemoveMoneyFromWalletDB(playerId, amountToDecimal);
                return new {success = true, message = "Money Withdrawn Successfully."};
            }
        }
    }


    public async Task<bool> IsSqlServerAvailableAsync()
    {
        return await _playerRepository.IsSqlServerRunning();
    }


    /* PRIVATE METHODS */


    private static async Task AssignPlayerCookie(HttpContext httpContext, int id, string email, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
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

    private int GetCurrentPlayerId(HttpContext httpContext)
    {
        string? playerId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrWhiteSpace(playerId))
        {
            return -1;
        }
        else
        {
            return Int32.Parse(playerId);
        }
    }

    private static bool VerifyPlayerFields(PlayerViewModel playerVm)
    {
        if (!IsValidEmail(playerVm.Email) || !IsValidUsername(playerVm.Username) || !IsValidPassword(playerVm.Password))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private static bool IsValidEmail(string email)
    {
        if (email.Length < 3)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private static bool IsValidUsername(string username)
    {
        if (username.Length < 3)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private static bool IsValidPassword(string password)
    {
        if (password.Length < 3)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
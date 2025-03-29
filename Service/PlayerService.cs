using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Distributed;
using MinesGame.Models;
using MinesGame.Repository;
using MinesGame.ViewModels;

namespace MinesGame.Service;

public class PlayerService : IPlayerService
{

    public readonly IPlayerRepository _playerRepository;
    public readonly IDistributedCache _cache;
    public PlayerService(IPlayerRepository _playerRepo, IDistributedCache cache)
    {
        _playerRepository = _playerRepo;
        _cache = cache;
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
            var dummyWallet = new WalletDisplayViewModel { };
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
            return new { success = false, message = "Player do not exists." };
        }
        else
        {
            decimal amountToDecimal = Convert.ToDecimal(amountToAdd);
            decimal playerBalance = await _playerRepository.GetPlayerBalanceDB(playerId);
            if (amountToDecimal < 100)
            {
                return new { success = false, message = "Cannot deposit less than 100rs." };
            }
            else if (playerBalance > 10000)
            {
                return new { success = false, message = "Wallet limit exceeds (10000rs)." };
            }
            else
            {
                await _playerRepository.AddMoneyToWalletDB(playerId, amountToDecimal);
                return new { success = true, message = "Money Deposited Successfully." };
            }
        }
    }

    public async Task<object> RemoveMoneyFromWallet(HttpContext httpContext, string amountToRemove)
    {
        int playerId = GetCurrentPlayerId(httpContext);
        if (playerId == -1)
        {
            return new { success = false, message = "Player do not exists." };
        }
        else
        {
            decimal amountToDecimal = Convert.ToDecimal(amountToRemove);
            decimal playerBalance = await _playerRepository.GetPlayerBalanceDB(playerId);
            if (amountToDecimal < 100)
            {
                return new { success = false, message = "Money Withdraw Error." };
            }
            else if (amountToDecimal > playerBalance)
            {
                return new { success = false, message = "Withdraw amount cannot exceed wallet balance." };
            }
            else
            {
                await _playerRepository.RemoveMoneyFromWalletDB(playerId, amountToDecimal);
                return new { success = true, message = "Money Withdrawn Successfully." };
            }
        }
    }

    public async Task<object> StartGameSession(HttpContext httpContext, int betAmount, int bombsCount)
    {
        int playerId = GetCurrentPlayerId(httpContext);
        bool isBettingAmountValid = await _playerRepository.BettingAmountValidateDB(playerId, betAmount);
        if (playerId == -1)
        {
            return new { success = false, message = "User not found." };
        }
        else if (isBettingAmountValid == false || bombsCount > 24 || bombsCount <= 0)
        {
            return new { success = false, message = "Incorrect fields." };
        }
        else
        {

            // Step 1: Generate Bombs position
            string getBombsPosition = GenerateBombsPosition(bombsCount);

            // Step 1: Create new Game Entry in DB first
            var gameId = await _playerRepository.CreateNewGameDB(playerId, betAmount, bombsCount, getBombsPosition);

            decimal getMultiplier = GetMultiplier(bombsCount);
            var gameLogicData = new
            {
                BombsPosition = getBombsPosition,
                Multiplier = getMultiplier
            };
            string gameLogicDataJSON = JsonSerializer.Serialize(gameLogicData);

            // Step 3: Store game logic in distributed cache (with proper expiration options)
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20)
            };

            await _cache.SetStringAsync($"GameLogic_{gameId}", gameLogicDataJSON, cacheOptions);

            // Step 4: Store the game ID in Session
            httpContext.Session.SetString("CurrentGameId", gameId.ToString());

            // Step 5: Return success response
            return new { success = true, message = "Game started successfully." };
        }
    }

    private string GenerateBombsPosition(int minesCount)
    {
        List<int> allPositions = Enumerable.Range(1, 25).ToList();
        ShuffleList(allPositions);

        StringBuilder minesPositions = new();
        for (int i = 0; i < minesCount; i++)
        {
            minesPositions.Append(allPositions[i]);
            if (i < minesCount - 1) minesPositions.Append(",");
        }
        return minesPositions.ToString();
    }

    private void ShuffleList(List<int> list)
    {
        Random random = new();
        int lastIndex = list.Count - 1;
        while (lastIndex > 0)
        {
            int randomIndex = random.Next(0, lastIndex + 1); // +1 because max value of random.Next is exclusive
            // Swap elements
            (list[randomIndex], list[lastIndex]) = (list[lastIndex], list[randomIndex]);
            lastIndex--;
        }
    }

    private decimal GetMultiplier(int minesCount)
    {
        switch (minesCount)
        {
            case 1: return 1.3M;
            case 2: return 1.6M;
            case 3: return 2M;
            case 4: return 2.4M;
            case 5: return 2.9M;
            case 6: return 3.5M;
            case 7: return 4M;
            case 8: return 4.5M;
            case 9: return 5.1M;
            case 10: return 5.7M;
            case 11: return 6.4M;
            case 12: return 7.5M;
            case 13: return 8.9M;
            case 14: return 10M;
            case 15: return 11.1M;
            case 16: return 12.4M;
            case 17: return 13.8M;
            case 18: return 15.5M;
            case 19: return 17.5M;
            case 20: return 19M;
            case 21: return 21M;
            case 22: return 23.5M;
            case 23: return 26M;
            case 24: return 30M;

            default: return 0M;
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
        if (string.IsNullOrWhiteSpace(playerId))
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
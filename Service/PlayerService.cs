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
        bool arePlayerFieldsValid = VerifyPlayerFields(playerVm);
        if (playerId == -1 || arePlayerFieldsValid == false)
        {
            return false;
        }
        else
        {
            bool isExistingUsernameOrEmail = await _playerRepository.CheckUsernameOrEmailExists(playerVm.Username, playerVm.Email, playerId);
            if (isExistingUsernameOrEmail == true)
            {
                return false;
            }
            else
            {
                await _playerRepository.UpdatePlayerDetailsAsync(playerId, playerVm);
                return true;
            }
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

    public async Task<object> StartGameSession(HttpContext httpContext, string betAmount, int bombsCount)
    {
        int playerId = GetCurrentPlayerId(httpContext);
        decimal betAmountConverted = Convert.ToDecimal(betAmount);
        bool isBettingAmountValid = await _playerRepository.BettingAmountValidateDB(playerId, betAmountConverted);

        if (playerId == -1)
        {
            return new { success = false, message = "User not found." };
        }
        else if (isBettingAmountValid == false || betAmountConverted < 10)
        {
            return new { success = false, message = "Betting amount lower than wallet. And greater than 10rs." };
        }
        else if (bombsCount < 1 || bombsCount > 24)
        {
            return new { success = false, message = "Invalid bombs count." };
        }
        else
        {
            // Step 1: Create new Game Entry in DB
            var gameId = await _playerRepository.CreateNewGameDB(playerId, betAmountConverted, bombsCount);

            // Step 2: Generate Bombs position and Multiplier
            string getBombsPosition = GenerateBombsPosition(bombsCount);
            decimal getMultiplier = Convert.ToDecimal(GetMultiplier(bombsCount));

            // Step 3a: Create game logic
            var gameLogicData = new
            {
                BombsPosition = getBombsPosition,
                Multiplier = getMultiplier
            };
            string gameLogicDataJSON = JsonSerializer.Serialize(gameLogicData);

            // Step 3b: Store game logic in distributed cache (with proper expiration options)
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20)
            };

            await _cache.SetStringAsync($"GameLogic_{gameId}", gameLogicDataJSON, cacheOptions);

            // Step 4: Store the game ID in Session
            httpContext.Session.SetString("GameId", gameId.ToString());

            // Step 5: Return success response
            return new { success = true, message = "Game started successfully." };
        }
    }

    public async Task<object> TileClickUpdateSession(HttpContext httpContext, int tilePosition)
    {
        string? gameId = httpContext.Session.GetString("GameId");
        if (gameId == null)
        {
            return new { success = false, message = "Game do not exists." };
        }
        else
        {
            var getGameLogic = await _cache.GetStringAsync($"GameLogic_{gameId}");
            if (getGameLogic == null)
            {
                return new { success = false, message = "Internal Cache error." };
            }
            else
            {
                bool isExistingClick = await _playerRepository.IsExistingTileClickedDB(int.Parse(gameId), tilePosition);
                if (isExistingClick) return new { success = false, message = "Tile already revealed." };

                var getGameLogicValues = JsonSerializer.Deserialize<GameLogicData>(getGameLogic);
                string bombsPosition = getGameLogicValues!.BombsPosition;
                List<int> bombPositionsList = bombsPosition.Split(',').Select(int.Parse).ToList();
                bool isBombDetected = bombPositionsList.Contains(tilePosition);

                if (isBombDetected == true)
                {
                    await _playerRepository.UpdateGamePlayerLostDB(int.Parse(gameId));
                    await EndGameSession(httpContext);
                    return new { diamond = false, message = "Bomb detected. Game Lost!" };
                }
                else
                {
                    decimal multiplier = getGameLogicValues.Multiplier;
                    await _playerRepository.UpdateGameDiamondClickedDB(int.Parse(gameId), multiplier, tilePosition);
                    decimal getUpdatedProfit = await _playerRepository.GetCurrentGameProfitDB(int.Parse(gameId));

                    return new { diamond = true, profit = getUpdatedProfit, message = "Diamond revealed. You're Safe." };
                }
            }
        }
    }

    public async Task<object> CashoutGameSession(HttpContext httpContext)
    {
        string? gameId = httpContext.Session.GetString("GameId");
        if (gameId == null)
        {
            return new { success = false, message = "Invalid request." };
        }
        else
        {
            int playerId = GetCurrentPlayerId(httpContext);
            if (playerId == -1)
            {
                return new { success = false, message = "User not found." };
            }
            else
            {
                await _playerRepository.UpdateGamePlayerWonDB(playerId, int.Parse(gameId));
                await EndGameSession(httpContext);
                return new { success = true, message = "Cashout done. You won." };
            }
        }
    }

    public async Task<object> GetActiveGameSession(HttpContext httpContext)
    {
        string? gameId = httpContext.Session.GetString("GameId");
        if (gameId == null)
        {
            return new { success = false, message = "No game found." };
        }
        else
        {
            string tilesPosition = await _playerRepository.GetCurrentGameTilesPosition(int.Parse(gameId));
            decimal profitDisplay = await _playerRepository.GetCurrentGameProfitDB(int.Parse(gameId));
            decimal betAmountDisplay = await _playerRepository.GetCurrentGameBetAmountDB(int.Parse(gameId));
            int minesSelectedDisplay = await _playerRepository.GetCurrentGameMinesSelectedDB(int.Parse(gameId));
            return new { success = true, profitDisplay, tilesPosition, betAmountDisplay, minesSelectedDisplay, message = "Existing game found. Tiles filled." };
        }
    }

    private async Task EndGameSession(HttpContext httpContext)
    {
        string? gameId = httpContext.Session.GetString("GameId");
        await _cache.RemoveAsync($"GameLogic_{gameId}");
        httpContext.Session.Remove("GameId");
    }

    private decimal GetMultiplier(int minesCount)
    {
        switch (minesCount)
        {
            case 1: return 1.05M;
            case 2: return 1.10M;
            case 3: return 1.35M;
            case 4: return 1.80M;
            case 5: return 2.50M;
            case 6: return 3.20M;
            case 7: return 3.90M;
            case 8: return 4.60M;
            case 9: return 5.40M;
            case 10: return 6.30M;
            case 11: return 7.30M;
            case 12: return 8.40M;
            case 13: return 9.60M;
            case 14: return 11.01M;
            case 15: return 12.50M;
            case 16: return 14.10M;
            case 17: return 16.02M;
            case 18: return 18.20M;
            case 19: return 20.60M;
            case 20: return 23.30M;
            case 21: return 26.20M;
            case 22: return 29.50M;
            case 23: return 33.20M;
            case 24: return 37.50M;
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
}
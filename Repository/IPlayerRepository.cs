using MinesGame.Models;
using MinesGame.ViewModels;

namespace MinesGame.Repository;

public interface IPlayerRepository
{
    Task AddPlayerAsync(Player player);
    Task UpdatePlayerDetailsAsync(int playerId, PlayerViewModel player);
    Task RemovePlayerAsync(Player player);

    Task<bool> CheckUsernameOrEmailExists(string username, string email, int playerId=-1);

    Task<Player?> GetPlayerAsync(string username, string email);

    Task<PlayerViewModel> GetDummyPlayer(int playerId);
    Task<WalletDisplayViewModel> GetDummyWallet(int playerId);

    Task<decimal> GetPlayerBalanceDB(int playerId);
    Task AddMoneyToWalletDB(int playerId, decimal amount);
    Task RemoveMoneyFromWalletDB(int playerId, decimal amount);

    Task<int> CreateNewGameDB(int playerId, decimal betAmount, int minesCount);
    Task UpdateGameDiamondClickedDB(int gameId, decimal multiplier, int tilePosition);
    Task UpdateGamePlayerWonDB(int playerId, int gameId);
    Task UpdateGamePlayerLostDB(int gameId);
    Task<decimal> GetCurrentGameBetAmountDB(int gameId);
    Task<int> GetCurrentGameMinesSelectedDB(int gameId);
    Task<decimal> GetCurrentGameProfitDB(int gameId);
    Task<string> GetCurrentGameTilesPosition(int gameId);
    Task<bool> IsExistingTileClickedDB(int gameId, int tilePosition);
    Task<bool> BettingAmountValidateDB(int playerId, decimal betAmount);

    Task<bool> IsSqlServerRunning();
    Task SaveToDbAsync();
}
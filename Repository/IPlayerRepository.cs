using MinesGame.Models;
using MinesGame.ViewModels;

namespace MinesGame.Repository;

public interface IPlayerRepository
{
    Task AddPlayerAsync(Player player);
    Task UpdatePlayerDetailsAsync(int playerId, PlayerViewModel player);
    Task RemovePlayerAsync(Player player);

    Task<bool> CheckUsernameOrEmailExists(string username, string email);

    Task<Player?> GetPlayerAsync(string username, string email);

    Task<PlayerViewModel> GetDummyPlayer(int playerId);
    Task<WalletDisplayViewModel> GetDummyWallet(int playerId);

    Task<decimal> GetPlayerBalanceDB(int playerId);
    Task AddMoneyToWalletDB(int playerId, decimal amount);
    Task RemoveMoneyFromWalletDB(int playerId, decimal amount);

    Task<int> CreateNewGameDB(int playerId, int betAmount, int minesCount, string tilePositions);

    Task<bool> BettingAmountValidateDB(int playerId, int betAmount);

    Task<bool> IsSqlServerRunning();
    Task SaveToDbAsync();
}
using MinesGame.Models;
using MinesGame.ViewModels;

namespace MinesGame.Repository;

public interface IPlayerRepository
{
    // 1) Players
    Task AddPlayerAsync(Player player);
    Task UpdatePlayerDetailsAsync(int playerId, PlayerViewModel player);
    Task RemovePlayerAsync(Player player);
    Task<bool> CheckUsernameOrEmailExists(string username, string email);
    Task<Player?> GetPlayerAsync(string username, string email);
    Task<PlayerViewModel> GetDummyPlayer(int playerId);
    Task<bool> IsSqlServerRunning();
    Task SaveToDbAsync();
}
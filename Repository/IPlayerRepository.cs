using MinesGame.Models;

namespace MinesGame.Repository;

public interface IPlayerRepository
{
    // 1) Players
    Task AddPlayerAsync(Player player);
    Task UpdatePlayerAsync(Player player);
    Task RemovePlayerAsync(Player player);
    Task<bool> IsExisitingPlayer(string username, string email);
    Task<Player?> GetPlayerAsync(string username_email);
    Task<bool> IsSqlServerRunning();
    Task SaveToDbAsync();
}
using MinesGame.Models;

namespace MinesGame.Repository;

public interface IGameRepository
{
    // 1) Players
    Task AddPlayerAsync(Player player);
    Task UpdatePlayerAsync(Player player);
    Task RemovePlayerAsync(Player player);

    Task SaveToDbAsync();
}
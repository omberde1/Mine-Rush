using MinesGame.Models;

namespace MinesGame.Service;

public interface IGameService
{
    Task<bool> RegisterPlayerAsync(Player player);
    Task<bool> LoginPlayerAsync(string username, string password);
    Task<bool> EditPlayerAsync(Player player);
}
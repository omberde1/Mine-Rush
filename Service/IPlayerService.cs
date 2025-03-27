using MinesGame.ViewModels;

namespace MinesGame.Service;

public interface IPlayerService
{
    Task<bool> RegisterPlayerAsync(PlayerViewModel player);
    Task<bool> LoginPlayerAsync(HttpContext httpContext, string username_email, string password);
    Task<bool> IsSqlServerAvailableAsync();
    // Task<bool> EditPlayerAsync(PlayerViewModel player);
}
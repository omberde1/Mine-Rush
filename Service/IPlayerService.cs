using MinesGame.ViewModels;

namespace MinesGame.Service;

public interface IPlayerService
{
    Task<bool> RegisterPlayerAsync(PlayerViewModel player);
    Task<bool> LoginPlayerAsync(HttpContext httpContext, string username_email, string password);
    Task<bool> EditPlayerAsync(HttpContext httpContext, PlayerViewModel player);
    Task<PlayerViewModel> CreateDummyPlayer(HttpContext httpContext);
    Task<WalletDisplayViewModel> CreateDummyPlayerWallet(HttpContext httpContext);
    Task<object> AddMoneyToWallet(HttpContext httpContext,string amountToAdd);
    Task<object> RemoveMoneyFromWallet(HttpContext httpContext,string amountToRemove);
    Task<bool> IsSqlServerAvailableAsync();
}
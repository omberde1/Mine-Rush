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

    Task<object> GetActiveGameSession(HttpContext httpContext);
    Task<object> StartGameSession(HttpContext httpContext, string betAmount, int minesCount);
    Task<object> TileClickUpdateSession(HttpContext httpContext, int tilePosition);
    Task<object> CashoutGameSession(HttpContext httpContext);

    Task<bool> IsSqlServerAvailableAsync();
}
using MinesGame.Models;

namespace MinesGame.Service;

public class PlayerService : IGameService
{

    public readonly IGameService _playerRepository;
    public PlayerService(IGameService _playerRepo)
    {
        _playerRepository = _playerRepo;
    }

    public async Task<bool> RegisterPlayerAsync(Player player)
    {
        return false;
    }

    public async Task<bool> LoginPlayerAsync(string username, string password)
    {
        return false;
    }

    public async Task<bool> EditPlayerAsync(Player player)
    {
        return false;
    }

}
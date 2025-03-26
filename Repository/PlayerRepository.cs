using MinesGame.Data;
using MinesGame.Models;

namespace MinesGame.Repository;

public class PlayerRepository : IGameRepository
{

    public readonly AppDbContext _context;
    public PlayerRepository(AppDbContext ctx)
    {
        _context = ctx;
    }

    public async Task AddPlayerAsync(Player player)
    {
        await _context.AddAsync(player);
        await SaveToDbAsync();
    }

    public async Task UpdatePlayerAsync(Player player)
    {
        _context.Update(player);
        await SaveToDbAsync();
    }

    public async Task RemovePlayerAsync(Player player)
    {
        _context.Remove(player);
        await SaveToDbAsync();
    }

    public async Task SaveToDbAsync()
    {
        await _context.SaveChangesAsync();
    }

}
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MinesGame.Data;
using MinesGame.Models;

namespace MinesGame.Repository;

public class PlayerRepository : IPlayerRepository
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

    public async Task<bool> IsExisitingPlayer(string username, string email)
    {
        return await _context.Table_Player.AnyAsync(p => p.Username == username || p.Email == email);
    }

    public async Task<Player?> GetPlayerAsync(string username_email)
    {
        return await _context.Table_Player.FirstOrDefaultAsync(p => p.Username == username_email || p.Email == username_email);
    }

    public async Task<bool> IsSqlServerRunning()
    {
        var connectionString = _context.Database.GetDbConnection().ConnectionString;
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(); // opening the connection
                return true; // opened so SQL Server is available
            }
        }
        catch
        {
            return false; // if not, then SQL Server is down
        }
    }

    public async Task SaveToDbAsync()
    {
        await _context.SaveChangesAsync();
    }

}
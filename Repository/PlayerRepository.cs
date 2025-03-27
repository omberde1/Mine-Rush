using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MinesGame.Data;
using MinesGame.Models;
using MinesGame.ViewModels;

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

    public async Task UpdatePlayerDetailsAsync(int playerId, PlayerViewModel player)
    {
        var currentPlayer = await _context.Table_Player.FindAsync(playerId);

        currentPlayer!.Username = player.Username;
        currentPlayer.Email = player.Email;
        currentPlayer.Password = player.Password;

        _context.Table_Player.Update(currentPlayer); // Now EF tracks only changed fields
        await SaveToDbAsync();
    }

    public async Task RemovePlayerAsync(Player player)
    {
        _context.Remove(player);
        await SaveToDbAsync();
    }

    public async Task<bool> CheckUsernameOrEmailExists(string username, string email)
    {
        return await _context.Table_Player.AnyAsync(p => p.Username == username || p.Email == email);
    }

    public async Task<Player?> GetPlayerAsync(string username, string email)
    {
        // Either returns null or player if found
        return await _context.Table_Player
        .FirstOrDefaultAsync(p => p.Username == username || p.Email == email);
    }

    public async Task<PlayerViewModel> GetDummyPlayer(int playerId)
    {
        var realPlayer = await _context.Table_Player.FindAsync(playerId);
        var newDummyplayer = new PlayerViewModel
        {
            Username = realPlayer!.Username,
            Email = realPlayer.Email,
            Password = realPlayer.Password
        };
        return newDummyplayer;
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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        var newDummyPlayer = new PlayerViewModel
        {
            Username = realPlayer!.Username,
            Email = realPlayer.Email,
            Password = realPlayer.Password
        };
        return newDummyPlayer;
    }

    public async Task<WalletDisplayViewModel> GetDummyWallet(int playerId)
    {
        var realPlayer = await _context.Table_Player.FindAsync(playerId);

        var transactions = await _context.Table_Transaction
        .Where(t => t.PlayerId == playerId)
        .OrderByDescending(t => t.CreatedAt) // Order transactions from newest to oldest
        .Take(10)
        .ToListAsync();

        if (transactions.IsNullOrEmpty())
        {
            return new WalletDisplayViewModel
            {
                Username = realPlayer!.Username,
                CurrentBalance = realPlayer.Balance,
                AllRecentTransactions = new List<WalletActionViewModel>()
            };
        }
        else
        {
            // Running select separately for readability
            var getAllRealPlayerTransactions = transactions
            .Select(t => new WalletActionViewModel
            {
                UID = t.TransactionUID,
                Type = t.Type switch
                {
                    TransactionType.Deposit => "Deposit",
                    TransactionType.Withdraw => "Withdraw",
                    _ => "Unknown"
                },
                Amount = t.Amount,
                Status = t.Status switch
                {
                    TransactionStatus.Pending => "Pending",
                    TransactionStatus.Completed => "Completed",
                    TransactionStatus.Failed => "Failed",
                    _ => "Unknown"
                },
                MadeAt = t.CreatedAt
            })
            .ToList();

            return new WalletDisplayViewModel
            {
                Username = realPlayer!.Username,
                CurrentBalance = realPlayer!.Balance,
                AllRecentTransactions = getAllRealPlayerTransactions
            };
        }
    }

    public async Task<decimal> GetPlayerBalanceDB(int playerId)
    {
        var currentPlayerBalance = await _context.Table_Player.FindAsync(playerId);
        return currentPlayerBalance!.Balance;
    }

    public async Task AddMoneyToWalletDB(int playerId, decimal amount)
    {
        var currentPlayer = await _context.Table_Player.FindAsync(playerId);
        currentPlayer!.Balance += amount;
        _context.Table_Player.Update(currentPlayer);

        string uniqueTransactionId = await GenerateUniqueTransactionId();
        var transaction = new Transaction
        {
            TransactionUID = uniqueTransactionId,
            PlayerId = playerId,
            Type = TransactionType.Deposit,
            Amount = amount,
            Status = TransactionStatus.Completed,
        };
        await _context.Table_Transaction.AddAsync(transaction);
        
        await SaveToDbAsync();
    }

    public async Task RemoveMoneyFromWalletDB(int playerId, decimal amount)
    {
        var currentPlayer = await _context.Table_Player.FindAsync(playerId);
        currentPlayer!.Balance -= amount;
        _context.Table_Player.Update(currentPlayer);

        string uniqueTransactionId = await GenerateUniqueTransactionId();
        var transaction = new Transaction
        {
            TransactionUID = uniqueTransactionId,
            PlayerId = playerId,
            Type = TransactionType.Withdraw,
            Amount = amount,
            Status = TransactionStatus.Completed,
        };
        await _context.Table_Transaction.AddAsync(transaction);

        await SaveToDbAsync();
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


    /* PRIVATE METHODS */

    private async Task<string> GenerateUniqueTransactionId()
    {
        string transactionId = string.Empty;
        do
        {
            Guid guid = Guid.NewGuid();
            transactionId = guid.ToString("N").Substring(0, 8); // "N" removes hyphens, then take the first 8 characters
        }
        while (await _context.Table_Transaction.AnyAsync(t => t.TransactionUID == transactionId));

        return transactionId;
    }

}
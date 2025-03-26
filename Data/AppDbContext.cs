using Microsoft.EntityFrameworkCore;
using MinesGame.Models;

namespace MinesGame.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Player> Table_Player { get; set; }
    public DbSet<Game> Table_Game { get; set; }
    public DbSet<Transaction> Table_Transaction { get; set; }
}
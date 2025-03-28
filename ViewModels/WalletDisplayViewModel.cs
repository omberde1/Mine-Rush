using System.ComponentModel.DataAnnotations;

namespace MinesGame.ViewModels
{
    public class WalletDisplayViewModel
    {
        [Required]
        public decimal CurrentBalance { get; set; } = 0;

        [Required]
        public decimal RecentTransactions { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace MinesGame.ViewModels
{
    public class WalletActionViewModel
    {
        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; } = 0;
    }
}

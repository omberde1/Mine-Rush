using System.ComponentModel.DataAnnotations;

namespace MinesGame.ViewModels
{
    public class PlayerMoneyViewModel
    {
        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public decimal Amount { get; set; } = 0;
    }
}

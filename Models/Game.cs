using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesGame.Models
{
    public enum GameStatus { Active, Won, Lost }

    public class Game
    {
        [Key]
        public int GameId { get; set; }

        public int PlayerId { get; set; }
        [ForeignKey("PlayerId")]
        public Player? Player { get; set; }

        [Required]
        public GameStatus Status { get; set; } = GameStatus.Active;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BetAmount { get; set; } = 0;

        [Required]
        public int MinesCount { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CashoutAmount { get; set; } = 0;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime? EndedAt { get; set; } = null;

        public string MinesPositions { get; set; } = string.Empty;
    }
}

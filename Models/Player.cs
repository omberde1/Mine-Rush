using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesGame.Models
{
    public class Player
    {
        [Key]
        public int PlayerId { get; set; }

        [Required] // column is NOT NULL
        [Column(TypeName = "VARCHAR(50)")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [Column(TypeName = "VARCHAR(100)")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;
    }
}

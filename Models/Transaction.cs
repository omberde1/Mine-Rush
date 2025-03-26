using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesGame.Models
{
    public enum TransactionType { Deposit, Withdraw }
    public enum TransactionStatus { Pending, Completed, Failed }

    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        public int PlayerId { get; set; }
<<<<<<< HEAD
        [ForeignKey("PlayerId")]
=======
        [ForeignKey("Player_Id")]
>>>>>>> 179f8ca167be3064e7568826beec9dfc6cf2bdbf
        public Player? Player { get; set; }
        
        [Required] 
        public TransactionType Type { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; } = 0;

        [Required] 
        public TransactionStatus Status { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

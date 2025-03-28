namespace MinesGame.ViewModels
{
    public class PlayerTransactions
    {
        public string TransactionUID { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public string TransactionStatus { get; set; } = string.Empty;
        public decimal TransactionAmount { get; set; } = 0;
        public DateTime TransactionDate { get; set; } = DateTime.Now;
    }
}
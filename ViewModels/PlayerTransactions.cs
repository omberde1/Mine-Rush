namespace MinesGame.ViewModels
{
    public class PlayerTransactions
    {
        public string Transaction_UID { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public decimal TransactionAmount { get; set; } = 0;
    }
}
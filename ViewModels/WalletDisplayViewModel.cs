namespace MinesGame.ViewModels
{
    public class WalletDisplayViewModel
    {
        public string Username { get; set; } = string.Empty;
        public decimal CurrentBalance { get; set; } = 0;
        public decimal NetProfit { get; set; } = 0;
        public List<WalletActionViewModel> AllRecentTransactions { get; set; } = [];
    }
}
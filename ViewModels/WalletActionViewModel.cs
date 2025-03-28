namespace MinesGame.ViewModels
{
    public class WalletActionViewModel
    {
        public string UID { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 0;
        public string Status { get; set; } = string.Empty;
        public DateTime MadeAt { get; set; } = DateTime.Now;
    }
}
using System.ComponentModel.DataAnnotations;

namespace MinesGame.ViewModels
{
    public class PlayerViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

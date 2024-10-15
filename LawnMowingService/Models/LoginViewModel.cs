

namespace LawnMowingService.Models // Ensure this matches your project structure
{
    public class LoginViewModel
    {
        public string? Email { get; set; } // Make Email nullable
        public string? Password { get; set; } // Make Password nullable
        public bool RememberMe { get; set; }
    }
}

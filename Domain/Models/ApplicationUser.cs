using Microsoft.AspNetCore.Identity;

namespace Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Role { get; set; }
        
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}

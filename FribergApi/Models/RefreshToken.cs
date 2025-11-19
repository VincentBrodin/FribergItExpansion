using System.ComponentModel.DataAnnotations;

namespace FribergApi.Models;

public class RefreshToken
{
    [Key]
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; } = false;
    public bool IsUsed { get; set; } = false;
    public DateTime Created { get; set; } = DateTime.UtcNow;
}

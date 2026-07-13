#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IMBP.App.Infrastructure.Models;

[Index(nameof(UserId), Name = "IX_UserSessions_UserId")]
[Index(nameof(RefreshTokenHash), Name = "IX_UserSessions_RefreshTokenHash")]
public class UserSession
{
    [Key]
    public Guid SessionId { get; set; }

    public Guid UserId { get; set; }

    [StringLength(64)]
    [Unicode(false)]
    public string RefreshTokenHash { get; set; } = null!;

    [StringLength(64)]
    [Unicode(false)]
    public string UserAgentHash { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ExpiresAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime LastRefreshedAt { get; set; }

    public bool IsRevoked { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
}

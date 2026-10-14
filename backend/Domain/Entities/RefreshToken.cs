using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; } // NULL, если токен ещё не отозван
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигационное свойство для связи с User
    public User User { get; set; } = null!;
}
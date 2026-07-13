using IMBP.App.Domain.Models;

namespace IMBP.App.Domain.Specifications
{
    public class SessionCreationResult
    {
        public required Guid SessionId { get; set; }
        public required AuthUser User { get; set; }
        public required string RefreshToken { get; set; }
        public required string AccessToken { get; set; }
        public required DateTime RefreshExpiresAt { get; set; }
    }

    public class SessionRefreshResult
    {
        public required Guid SessionId { get; set; }
        public required AuthUser User { get; set; }
        public required string RefreshToken { get; set; }
        public required string AccessToken { get; set; }
        public required DateTime RefreshExpiresAt { get; set; }
    }

    public interface ISessionService
    {
        Task<SessionCreationResult> CreateSessionAsync(AuthUser user, string userAgent, bool rememberMe, CancellationToken cancellationToken = default);
        Task<SessionRefreshResult?> RefreshSessionAsync(string refreshToken, string userAgent, CancellationToken cancellationToken = default);
        Task<bool> IsSessionValidAsync(Guid sessionId, string userAgent, CancellationToken cancellationToken = default);
        Task RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
        Task RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Guid?> GetSessionIdByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}

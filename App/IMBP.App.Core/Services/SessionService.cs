using System.Security.Cryptography;
using System.Text;
using IMBP.App.Domain.Models;
using IMBP.App.Domain.Settings;
using IMBP.App.Domain.Specifications;
using IMBP.App.Infrastructure;
using IMBP.App.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IMBP.App.Core.Services
{
    internal class SessionService(
        PortalContext dbContext,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtOptions) : ISessionService
    {
        private readonly JwtSettings _jwtSettings = jwtOptions.Value;

        public async Task<SessionCreationResult> CreateSessionAsync(
            AuthUser user,
            string userAgent,
            bool rememberMe,
            CancellationToken cancellationToken = default)
        {
            if (_jwtSettings.EnforceSingleSession)
            {
                await RevokeAllUserSessionsAsync(user.UserId, cancellationToken);
            }

            var refreshToken = GenerateRefreshToken();
            var now = DateTime.UtcNow;
            var refreshExpirationDays = rememberMe
                ? _jwtSettings.RefreshTokenExpirationDays
                : 1;
            var refreshExpiresAt = now.AddDays(refreshExpirationDays);

            var session = new UserSession
            {
                SessionId = Guid.NewGuid(),
                UserId = user.UserId,
                RefreshTokenHash = HashToken(refreshToken),
                UserAgentHash = HashUserAgent(userAgent),
                CreatedAt = now,
                LastRefreshedAt = now,
                ExpiresAt = refreshExpiresAt,
                IsRevoked = false,
            };

            dbContext.UserSessions.Add(session);
            await dbContext.SaveChangesAsync(cancellationToken);

            var accessToken = tokenService.GenerateAccessToken(user, session.SessionId);

            return new SessionCreationResult
            {
                SessionId = session.SessionId,
                User = user,
                RefreshToken = refreshToken,
                AccessToken = accessToken,
                RefreshExpiresAt = refreshExpiresAt,
            };
        }

        public async Task<SessionRefreshResult?> RefreshSessionAsync(
            string refreshToken,
            string userAgent,
            CancellationToken cancellationToken = default)
        {
            var session = await GetActiveSessionByRefreshTokenAsync(refreshToken, cancellationToken);
            if (session is null)
            {
                return null;
            }

            if (!ValidateUserAgent(session, userAgent))
            {
                await RevokeSessionAsync(session.SessionId, cancellationToken);
                return null;
            }

            var newRefreshToken = GenerateRefreshToken();
            var now = DateTime.UtcNow;

            session.RefreshTokenHash = HashToken(newRefreshToken);
            session.LastRefreshedAt = now;
            await dbContext.SaveChangesAsync(cancellationToken);

            var authUser = session.User.ToAuthUser();
            var accessToken = tokenService.GenerateAccessToken(authUser, session.SessionId);

            return new SessionRefreshResult
            {
                SessionId = session.SessionId,
                User = authUser,
                RefreshToken = newRefreshToken,
                AccessToken = accessToken,
                RefreshExpiresAt = session.ExpiresAt,
            };
        }

        public async Task<bool> IsSessionValidAsync(
            Guid sessionId,
            string userAgent,
            CancellationToken cancellationToken = default)
        {
            var session = await dbContext.UserSessions
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    entity => entity.SessionId == sessionId && !entity.IsRevoked,
                    cancellationToken);

            if (session is null || session.ExpiresAt <= DateTime.UtcNow)
            {
                return false;
            }

            return ValidateUserAgent(session, userAgent);
        }

        public async Task RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            var session = await dbContext.UserSessions
                .FirstOrDefaultAsync(entity => entity.SessionId == sessionId, cancellationToken);

            if (session is null)
            {
                return;
            }

            session.IsRevoked = true;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var sessions = await dbContext.UserSessions
                .Where(entity => entity.UserId == userId && !entity.IsRevoked)
                .ToListAsync(cancellationToken);

            if (sessions.Count == 0)
            {
                return;
            }

            foreach (var session in sessions)
            {
                session.IsRevoked = true;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Guid?> GetSessionIdByRefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken = default)
        {
            var session = await GetActiveSessionByRefreshTokenAsync(refreshToken, cancellationToken);
            return session?.SessionId;
        }

        private async Task<UserSession?> GetActiveSessionByRefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken)
        {
            var refreshTokenHash = HashToken(refreshToken);

            var session = await dbContext.UserSessions
                .Include(entity => entity.User)
                .FirstOrDefaultAsync(
                    entity => entity.RefreshTokenHash == refreshTokenHash && !entity.IsRevoked,
                    cancellationToken);

            if (session is null)
            {
                return null;
            }

            if (session.ExpiresAt <= DateTime.UtcNow)
            {
                session.IsRevoked = true;
                await dbContext.SaveChangesAsync(cancellationToken);
                return null;
            }

            return session;
        }

        private static string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }

        internal static string HashToken(string token)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(hash);
        }

        internal static string HashUserAgent(string userAgent)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(userAgent));
            return Convert.ToHexString(hash);
        }

        private static bool ValidateUserAgent(UserSession session, string userAgent)
        {
            return session.UserAgentHash == HashUserAgent(userAgent);
        }
    }
}

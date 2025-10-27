using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Admin.Commands;
using LinkShortener.Application.Features.Admin.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LinkShortener.Application.Features.Admin.Handlers
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, List<UserListResponse>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserListResponse>> Handle(GetAllUsersQuery request, CancellationToken ct)
        {
            var users = await _userRepository.GetAllAsync(request.Page, request.PageSize, ct);
            
            return users.Select(u => new UserListResponse(
                u.Id,
                u.Username,
                u.Email,
                u.Status,
                u.Roles.Select(r => r.ToString()).ToList(),
                u.CreatedOnUtc,
                u.LastLoginAt,
                u.IsEmailVerified,
                u.AuthProvider.ToString()
            )).ToList();
        }
    }

    public class GetUserDetailHandler : IRequestHandler<GetUserDetailQuery, UserDetailResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;

        public GetUserDetailHandler(IUserRepository userRepository, ISessionRepository sessionRepository)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task<UserDetailResponse> Handle(GetUserDetailQuery request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, ct);
            if (user == null)
                throw new KeyNotFoundException($"User not found: {request.UserId}");

            var activeSessions = await _sessionRepository.GetActiveSessionCountAsync(user.Id, ct);

            return new UserDetailResponse(
                user.Id,
                user.Username,
                user.Email,
                user.Status,
                user.Roles.Select(r => r.ToString()).ToList(),
                user.CreatedOnUtc,
                user.LastLoginAt,
                user.IsEmailVerified,
                user.EmailVerifiedAt,
                user.AuthProvider.ToString(),
                user.ExternalProviderId,
                user.SuspendedAt,
                user.SuspensionReason,
                activeSessions,
                user.Links.Count
            );
        }
    }

    public class SuspendUserHandler : IRequestHandler<SuspendUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<SuspendUserHandler> _logger;

        public SuspendUserHandler(IUserRepository userRepository, ILogger<SuspendUserHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(SuspendUserCommand request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, ct);
            if (user == null)
                throw new KeyNotFoundException($"User not found: {request.UserId}");

            user.Suspend(request.Reason);
            await _userRepository.SaveChangesAsync(ct);

            _logger.LogInformation("User {UserId} suspended. Reason: {Reason}", request.UserId, request.Reason);
            return Unit.Value;
        }
    }

    public class BanUserHandler : IRequestHandler<BanUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<BanUserHandler> _logger;

        public BanUserHandler(
            IUserRepository userRepository,
            ISessionRepository sessionRepository,
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<BanUserHandler> logger)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(BanUserCommand request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, ct);
            if (user == null)
                throw new KeyNotFoundException($"User not found: {request.UserId}");

            user.Ban(request.Reason);
            
            await _sessionRepository.EndAllUserSessionsAsync(request.UserId, ct);
            await _refreshTokenRepository.RevokeAllUserTokensAsync(request.UserId, ct);
            await _userRepository.SaveChangesAsync(ct);

            _logger.LogWarning("User {UserId} banned. Reason: {Reason}", request.UserId, request.Reason);
            return Unit.Value;
        }
    }

    public class UnsuspendUserHandler : IRequestHandler<UnsuspendUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UnsuspendUserHandler> _logger;

        public UnsuspendUserHandler(IUserRepository userRepository, ILogger<UnsuspendUserHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UnsuspendUserCommand request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, ct);
            if (user == null)
                throw new KeyNotFoundException($"User not found: {request.UserId}");

            user.Unsuspend();
            await _userRepository.SaveChangesAsync(ct);

            _logger.LogInformation("User {UserId} unsuspended", request.UserId);
            return Unit.Value;
        }
    }

    public class AddRoleHandler : IRequestHandler<AddRoleCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AddRoleHandler> _logger;

        public AddRoleHandler(IUserRepository userRepository, ILogger<AddRoleHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(AddRoleCommand request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, ct);
            if (user == null)
                throw new KeyNotFoundException($"User not found: {request.UserId}");

            user.AddRole(request.Role);
            await _userRepository.SaveChangesAsync(ct);

            _logger.LogInformation("Role {Role} added to user {UserId}", request.Role, request.UserId);
            return Unit.Value;
        }
    }

    public class RemoveRoleHandler : IRequestHandler<RemoveRoleCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RemoveRoleHandler> _logger;

        public RemoveRoleHandler(IUserRepository userRepository, ILogger<RemoveRoleHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(RemoveRoleCommand request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, ct);
            if (user == null)
                throw new KeyNotFoundException($"User not found: {request.UserId}");

            user.RemoveRole(request.Role);
            await _userRepository.SaveChangesAsync(ct);

            _logger.LogInformation("Role {Role} removed from user {UserId}", request.Role, request.UserId);
            return Unit.Value;
        }
    }

    public class GetUserSessionsHandler : IRequestHandler<GetUserSessionsQuery, List<SessionResponse>>
    {
        private readonly ISessionRepository _sessionRepository;

        public GetUserSessionsHandler(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<List<SessionResponse>> Handle(GetUserSessionsQuery request, CancellationToken ct)
        {
            var sessions = await _sessionRepository.GetAllByUserIdAsync(request.UserId, ct);
            
            return sessions.Select(s => new SessionResponse(
                s.Id,
                s.IpAddress,
                s.UserAgent,
                s.DeviceName,
                s.Location,
                s.CreatedAt,
                s.LastActivityAt,
                s.EndedAt,
                s.IsActive,
                s.Duration
            )).ToList();
        }
    }

    public class EndUserSessionHandler : IRequestHandler<EndUserSessionCommand, Unit>
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ILogger<EndUserSessionHandler> _logger;

        public EndUserSessionHandler(ISessionRepository sessionRepository, ILogger<EndUserSessionHandler> logger)
        {
            _sessionRepository = sessionRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(EndUserSessionCommand request, CancellationToken ct)
        {
            var session = await _sessionRepository.GetByIdAsync(request.SessionId, ct);
            if (session == null || session.UserId != request.UserId)
                throw new KeyNotFoundException("Session not found");

            session.End();
            await _sessionRepository.SaveChangesAsync(ct);

            _logger.LogInformation("Session {SessionId} ended for user {UserId}", request.SessionId, request.UserId);
            return Unit.Value;
        }
    }

    public class EndAllUserSessionsHandler : IRequestHandler<EndAllUserSessionsCommand, Unit>
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ILogger<EndAllUserSessionsHandler> _logger;

        public EndAllUserSessionsHandler(ISessionRepository sessionRepository, ILogger<EndAllUserSessionsHandler> logger)
        {
            _sessionRepository = sessionRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(EndAllUserSessionsCommand request, CancellationToken ct)
        {
            await _sessionRepository.EndAllUserSessionsAsync(request.UserId, ct);
            _logger.LogInformation("All sessions ended for user {UserId}", request.UserId);
            return Unit.Value;
        }
    }

    public class RevokeAllRefreshTokensHandler : IRequestHandler<RevokeAllRefreshTokensCommand, Unit>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<RevokeAllRefreshTokensHandler> _logger;

        public RevokeAllRefreshTokensHandler(
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<RevokeAllRefreshTokensHandler> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(RevokeAllRefreshTokensCommand request, CancellationToken ct)
        {
            await _refreshTokenRepository.RevokeAllUserTokensAsync(request.UserId, ct);
            _logger.LogInformation("All refresh tokens revoked for user {UserId}", request.UserId);
            return Unit.Value;
        }
    }
}

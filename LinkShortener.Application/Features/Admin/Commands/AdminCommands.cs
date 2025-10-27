using LinkShortener.Application.Features.Admin.DTOs;
using LinkShortener.Domain.Entities;
using MediatR;

namespace LinkShortener.Application.Features.Admin.Commands
{
    public record GetAllUsersQuery(int Page = 1, int PageSize = 50) : IRequest<List<UserListResponse>>;

    public record GetUserDetailQuery(Guid UserId) : IRequest<UserDetailResponse>;

    public record SuspendUserCommand(Guid UserId, string Reason) : IRequest<Unit>;

    public record BanUserCommand(Guid UserId, string Reason) : IRequest<Unit>;

    public record UnsuspendUserCommand(Guid UserId) : IRequest<Unit>;

    public record AddRoleCommand(Guid UserId, Role Role) : IRequest<Unit>;

    public record RemoveRoleCommand(Guid UserId, Role Role) : IRequest<Unit>;

    public record GetUserSessionsQuery(Guid UserId) : IRequest<List<SessionResponse>>;

    public record EndUserSessionCommand(Guid UserId, Guid SessionId) : IRequest<Unit>;

    public record EndAllUserSessionsCommand(Guid UserId) : IRequest<Unit>;

    public record RevokeAllRefreshTokensCommand(Guid UserId) : IRequest<Unit>;
}

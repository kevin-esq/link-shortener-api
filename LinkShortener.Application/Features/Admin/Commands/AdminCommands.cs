using LinkShortener.Application.Features.Admin.DTOs;
using LinkShortener.Domain.Entities;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;

namespace LinkShortener.Application.Features.Admin.Commands
{
    public record GetAllUsersQuery(int Page = 1, int PageSize = 50) : IQuery<List<UserListResponse>>;

    public record GetUserDetailQuery(Guid UserId) : IQuery<UserDetailResponse>;

    public record SuspendUserCommand(Guid UserId, string Reason) : ICommand;

    public record BanUserCommand(Guid UserId, string Reason) : ICommand;

    public record UnsuspendUserCommand(Guid UserId) : ICommand;

    public record AddRoleCommand(Guid UserId, Role Role) : ICommand;

    public record RemoveRoleCommand(Guid UserId, Role Role) : ICommand;

    public record GetUserSessionsQuery(Guid UserId) : IQuery<List<SessionResponse>>;

    public record EndUserSessionCommand(Guid UserId, Guid SessionId) : ICommand;

    public record EndAllUserSessionsCommand(Guid UserId) : ICommand;

    public record RevokeAllRefreshTokensCommand(Guid UserId) : ICommand;
}

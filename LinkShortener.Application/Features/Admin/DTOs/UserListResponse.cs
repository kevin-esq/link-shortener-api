using LinkShortener.Domain.Entities;

namespace LinkShortener.Application.Features.Admin.DTOs
{
    public record UserListResponse(
        Guid UserId,
        string Username,
        string Email,
        UserStatus Status,
        List<string> Roles,
        DateTime CreatedAt,
        DateTime? LastLoginAt,
        bool IsEmailVerified,
        string AuthProvider);

    public record UserDetailResponse(
        Guid UserId,
        string Username,
        string Email,
        UserStatus Status,
        List<string> Roles,
        DateTime CreatedAt,
        DateTime? LastLoginAt,
        bool IsEmailVerified,
        DateTime? EmailVerifiedAt,
        string AuthProvider,
        string? ExternalProviderId,
        DateTime? SuspendedAt,
        string? SuspensionReason,
        int ActiveSessionsCount,
        int TotalLinksCreated);

    public record SuspendUserRequest(string Reason);

    public record BanUserRequest(string Reason);

    public record SessionResponse(
        Guid SessionId,
        string IpAddress,
        string UserAgent,
        string? DeviceName,
        string? Location,
        DateTime CreatedAt,
        DateTime LastActivityAt,
        DateTime? EndedAt,
        bool IsActive,
        TimeSpan Duration);
}

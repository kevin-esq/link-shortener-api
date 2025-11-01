using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Analytics.DTOs;
using LinkShortener.Application.Features.Analytics.Queries;
using LiteBus.Queries.Abstractions;

namespace LinkShortener.Application.Features.Analytics.Handlers
{
    /// <summary>
    /// Handler for retrieving dashboard analytics for a user.
    /// </summary>
    public class GetUserDashboardQueryHandler : IQueryHandler<GetUserDashboardQuery, UserDashboardDto>
    {
        private readonly IAnalyticsRepository _analyticsRepository;
        private readonly IAnalyticsCacheService _cacheService;

        public GetUserDashboardQueryHandler(
            IAnalyticsRepository analyticsRepository,
            IAnalyticsCacheService cacheService)
        {
            _analyticsRepository = analyticsRepository;
            _cacheService = cacheService;
        }

        public async Task<UserDashboardDto> HandleAsync(GetUserDashboardQuery request, CancellationToken cancellationToken)
        {
            // Try to get from cache first
            var cacheKey = $"analytics:user:{request.UserId}:days:{request.DaysToAnalyze}";
            var cached = await _cacheService.GetAnalyticsAsync<UserDashboardDto>(cacheKey, cancellationToken);

            if (cached != null)
                return cached;

            // Get from database
            var dashboard = await _analyticsRepository.GetUserDashboardAsync(
                request.UserId,
                request.DaysToAnalyze,
                cancellationToken);

            // Cache for 5 minutes
            await _cacheService.SetAnalyticsAsync(
                cacheKey,
                dashboard,
                TimeSpan.FromMinutes(5),
                cancellationToken);

            return dashboard;
        }
    }
}

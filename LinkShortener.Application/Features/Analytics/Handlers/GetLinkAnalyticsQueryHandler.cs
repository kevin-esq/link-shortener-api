using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Analytics.DTOs;
using LinkShortener.Application.Features.Analytics.Queries;
using MediatR;

namespace LinkShortener.Application.Features.Analytics.Handlers
{
    /// <summary>
    /// Handler for retrieving comprehensive analytics for a specific link.
    /// </summary>
    public class GetLinkAnalyticsQueryHandler : IRequestHandler<GetLinkAnalyticsQuery, LinkAnalyticsDto?>
    {
        private readonly IAnalyticsRepository _analyticsRepository;
        private readonly IAnalyticsCacheService _cacheService;

        public GetLinkAnalyticsQueryHandler(
            IAnalyticsRepository analyticsRepository,
            IAnalyticsCacheService cacheService)
        {
            _analyticsRepository = analyticsRepository;
            _cacheService = cacheService;
        }

        public async Task<LinkAnalyticsDto?> Handle(GetLinkAnalyticsQuery request, CancellationToken cancellationToken)
        {
            // Try to get from cache first
            var cacheKey = $"analytics:link:{request.LinkId}:days:{request.DaysToAnalyze}";
            var cached = await _cacheService.GetAnalyticsAsync<LinkAnalyticsDto>(cacheKey, cancellationToken);

            if (cached != null)
                return cached;

            // Get from database
            var analytics = await _analyticsRepository.GetLinkAnalyticsAsync(
                request.LinkId, 
                request.DaysToAnalyze, 
                cancellationToken);

            // Cache for 5 minutes
            if (analytics != null)
            {
                await _cacheService.SetAnalyticsAsync(
                    cacheKey, 
                    analytics, 
                    TimeSpan.FromMinutes(5), 
                    cancellationToken);
            }

            return analytics;
        }
    }
}

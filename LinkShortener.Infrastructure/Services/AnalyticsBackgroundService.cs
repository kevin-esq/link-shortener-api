using LinkShortener.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LinkShortener.Infrastructure.Services
{
    /// <summary>
    /// Background service for processing analytics events asynchronously.
    /// Reduces latency on user redirects by processing analytics data in the background.
    /// Can be enhanced with message queues (RabbitMQ, Azure Service Bus, etc.) for production scale.
    /// </summary>
    public class AnalyticsBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AnalyticsBackgroundService> _logger;
        private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(30);

        public AnalyticsBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<AnalyticsBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Analytics Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessAnalyticsEventsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing analytics events");
                }

                await Task.Delay(_processingInterval, stoppingToken);
            }

            _logger.LogInformation("Analytics Background Service stopped");
        }

        private async Task ProcessAnalyticsEventsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var cacheService = scope.ServiceProvider.GetRequiredService<IAnalyticsCacheService>();

            // TODO: Implement batch processing of cached events
            // This is where you would:
            // 1. Retrieve batches of events from Redis queue
            // 2. Parse and enrich the data
            // 3. Bulk insert into the database
            // 4. Clear processed events from cache

            // Example implementation structure:
            /*
            var events = await cacheService.GetPendingEventsAsync(cancellationToken);
            if (events.Any())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IUrlRepository>();
                foreach (var eventBatch in events.Chunk(100))
                {
                    // Process batch
                    await repository.BulkInsertAccessesAsync(eventBatch, cancellationToken);
                }
            }
            */

            await Task.CompletedTask;
        }
    }
}

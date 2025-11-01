using LinkShortener.Application.Abstractions;
using LinkShortener.Application.Features.Url.Commands;
using LinkShortener.Domain.Entities;
using LiteBus.Commands.Abstractions;

namespace LinkShortener.Application.Features.Url.Handlers
{
    /// <summary>
    /// Handler for registering link access events with enhanced analytics data.
    /// Part of the LinkPulse analytics system.
    /// </summary>
    public class RegisterLinkAccessCommandHandler : ICommandHandler<RegisterLinkAccessCommand>
    {
        private readonly IUrlRepository _repository;
        private readonly IUserAgentParser _userAgentParser;
        private readonly IGeolocationService _geolocationService;

        public RegisterLinkAccessCommandHandler(
            IUrlRepository repository,
            IUserAgentParser userAgentParser,
            IGeolocationService geolocationService)
        {
            _repository = repository;
            _userAgentParser = userAgentParser;
            _geolocationService = geolocationService;
        }

        public async Task HandleAsync(RegisterLinkAccessCommand request, CancellationToken cancellationToken)
        {
            var link = await _repository.GetByIdAsync(request.LinkId, cancellationToken);
            if (link == null)
                throw new InvalidOperationException($"No existe un enlace con el ID {request.LinkId}");

            // Parse user agent for device and browser info
            var userAgentInfo = _userAgentParser.Parse(request.UserAgent);

            // Get geolocation data
            var geoInfo = await _geolocationService.GetLocationAsync(request.IpAddress, cancellationToken);

            // Create access event with full analytics data
            var access = new LinkAccess(
                linkId: request.LinkId,
                userId: request.UserId,
                ipAddress: request.IpAddress,
                userAgent: request.UserAgent,
                referer: request.Referer,
                country: geoInfo.Country,
                city: geoInfo.City,
                browser: userAgentInfo.Browser,
                browserVersion: userAgentInfo.BrowserVersion,
                operatingSystem: userAgentInfo.OperatingSystem,
                deviceType: userAgentInfo.DeviceType,
                deviceBrand: userAgentInfo.DeviceBrand
            );

            await _repository.AddAccessAsync(access, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}

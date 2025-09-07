using LinkShortener.Application.DTOs;
using LinkShortener.Application.Features.GetUrlInfo;
using LinkShortener.Application.Features.ShortenUrl.LinkShortener.Application.Features.ShortenUrl;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortener.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UrlController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UrlController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Shortens a long URL into a short code
        /// </summary>
        [HttpPost("shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Url))
                return BadRequest("URL is required.");

            var result = await _mediator.Send(new ShortenUrlCommand(
                request.Url,
                Request.Scheme,
                Request.Host.ToString()), cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Redirects to the original long URL
        /// </summary>
        [HttpGet("/s/{code}")]
        public async Task<IActionResult> RedirectToLongUrl(string code)
        {
            var result = await _mediator.Send(new GetUrlInfoQuery(code));

            if (result == null)
                return NotFound();

            return Redirect(result.OriginalUrl);
        }

        /// <summary>
        /// Gets information about a shortened URL (without redirecting)
        /// </summary>
        [HttpGet("info/{code}")]
        public async Task<IActionResult> GetInfo(string code, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetUrlInfoQuery(code), cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }
    }
}

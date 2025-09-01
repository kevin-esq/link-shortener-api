using LinkShortener.Application.Abstractions;
using LinkShortener.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LinkShortener.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShortenerController(IUrlShorteningAppService shorteningService) : ControllerBase
    {
        private readonly IUrlShorteningAppService _shorteningService = shorteningService;

        [HttpPost("shorten")]
        public async Task<IActionResult> Shorten([FromBody] ShortenUrlRequest request)
        {
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            {
                return BadRequest("The specified URL is invalid");
            }

            var shortUrl = await _shorteningService.ShortenUrlAsync(
                request.Url,
                Request.Scheme,
                Request.Host.ToString()
            );

            return Ok(shortUrl);
        }

        [HttpGet("/s/{code}")]
        public async Task<IActionResult> RedirectToLongUrl(string code)
        {
            var longUrl = await _shorteningService.GetLongUrlAsync(code);

            if (longUrl is null)
            {
                return NotFound();
            }

            return Redirect(longUrl);
        }
    }
}

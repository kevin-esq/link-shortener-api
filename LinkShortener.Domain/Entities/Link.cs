using LinkShortener.Domain.Common;

namespace LinkShortener.Domain.Entities
{
    public class Link : BaseEntity
    {
        private Link() { }
        public static Link Create(string longUrl, string code, Guid userId)
        {
            var link = new Link
            {
                Id = Guid.NewGuid(),
                LongUrl = ValidateLongUrl(longUrl),
                Code = ValidateCode(code),
                UserId = userId,
                CreatedOnUtc = DateTime.UtcNow
            };
            return link;
        }

        public string LongUrl { get; private set; } = string.Empty;
        public string Code { get; private set; } = string.Empty;
        public string ShortUrl { get; private set; } = string.Empty;
        public Guid UserId { get; private set; }
        public User? User { get; private set; }

        private readonly List<LinkAccess> _accesses = new();
        public IReadOnlyCollection<LinkAccess> Accesses => _accesses.AsReadOnly();

        public DateTime CreatedOnUtc { get; private set; }
        public DateTime? UpdatedOnUtc { get; private set; }

        public void OverrideShortUrlBase(string basePath)
        {
            if (string.IsNullOrWhiteSpace(basePath))
                throw new ArgumentException("Base path cannot be empty.", nameof(basePath));

            ShortUrl = $"{basePath.TrimEnd('/')}/{Code}";
            UpdatedOnUtc = DateTime.UtcNow;
        }

        #region Validations
        private static string ValidateLongUrl(string longUrl)
        {
            if (string.IsNullOrWhiteSpace(longUrl))
                throw new ArgumentException("Long URL cannot be empty.", nameof(longUrl));

            if (!Uri.TryCreate(longUrl, UriKind.Absolute, out _))
                throw new ArgumentException("Invalid URL format.", nameof(longUrl));

            return longUrl;
        }

        private static string ValidateCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be empty.", nameof(code));

            if (code.Length < 4 || code.Length > 20)
                throw new ArgumentException("Code must be between 4 and 20 characters.", nameof(code));

            return code.Trim();
        }

        public void AddAccess(Guid? userId, string ipAddress, string userAgent)
        {
            var access = new LinkAccess(Id, userId ?? Guid.Empty, ipAddress, userAgent);
            _accesses.Add(access);
        }
        #endregion
    }
}

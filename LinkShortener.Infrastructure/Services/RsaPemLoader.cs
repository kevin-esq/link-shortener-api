using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace LinkShortener.Infrastructure.Services
{
    /// <summary>
    /// Provides utility methods for loading and converting RSA keys from PEM-formatted strings.
    /// </summary>
    /// <remarks>
    /// This static helper class simplifies the process of loading RSA private and public keys
    /// from PEM files (commonly used in JWT signing and verification).
    /// It supports both <c>PRIVATE KEY</c> (PKCS#8) and <c>PUBLIC KEY</c> (SPKI) formats.
    /// </remarks>
    public static class RsaPemLoader
    {
        /// <summary>
        /// Loads an RSA private key from a PEM-encoded string.
        /// </summary>
        /// <param name="privatePem">A string containing a PEM-formatted private key.</param>
        /// <returns>An initialized <see cref="RSA"/> instance with the imported private key.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the provided PEM string does not contain a valid <c>PRIVATE KEY</c> block.
        /// </exception>
        /// <remarks>
        /// - Expected format: <c>-----BEGIN PRIVATE KEY----- ... -----END PRIVATE KEY-----</c>.<br/>
        /// - The private key is imported using <see cref="RSA.ImportPkcs8PrivateKey(ReadOnlySpan{byte}, out int)"/>.
        /// </remarks>
        public static RSA LoadPrivateKeyFromPem(string privatePem)
        {
            var pkcs8 = PemToBytes(privatePem, "PRIVATE KEY");
            var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(pkcs8, out _);
            return rsa;
        }

        /// <summary>
        /// Loads an RSA public key from a PEM-encoded string.
        /// </summary>
        /// <param name="publicPem">A string containing a PEM-formatted public key.</param>
        /// <returns>An initialized <see cref="RSA"/> instance with the imported public key.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the provided PEM string does not contain a valid <c>PUBLIC KEY</c> block.
        /// </exception>
        /// <remarks>
        /// - Expected format: <c>-----BEGIN PUBLIC KEY----- ... -----END PUBLIC KEY-----</c>.<br/>
        /// - The public key is imported using <see cref="RSA.ImportSubjectPublicKeyInfo(ReadOnlySpan{byte}, out int)"/>.
        /// </remarks>
        public static RSA LoadPublicKeyFromPem(string publicPem)
        {
            var spki = PemToBytes(publicPem, "PUBLIC KEY");
            var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(spki, out _);
            return rsa;
        }

        /// <summary>
        /// Converts a PEM-formatted string into a raw byte array.
        /// </summary>
        /// <param name="pem">The PEM string containing the encoded key.</param>
        /// <param name="label">The label used in the PEM header (e.g., "PRIVATE KEY", "PUBLIC KEY").</param>
        /// <returns>The decoded byte array of the base64-encoded key data.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the PEM format is invalid or the specified label is missing.
        /// </exception>
        /// <remarks>
        /// Uses a regular expression to locate and extract the key data between the
        /// <c>BEGIN</c> and <c>END</c> markers, stripping newlines and whitespace.
        /// </remarks>
        private static byte[] PemToBytes(string pem, string label)
        {
            var pattern = $"-----BEGIN {label}-----(.*?)-----END {label}-----";
            var m = Regex.Match(pem, pattern, RegexOptions.Singleline);

            if (!m.Success)
                throw new ArgumentException($"PEM format error: missing {label}");

            var base64 = m.Groups[1].Value.Replace("\r", "").Replace("\n", "").Trim();
            return Convert.FromBase64String(base64);
        }

        /// <summary>
        /// Creates a <see cref="RsaSecurityKey"/> instance from a PEM-encoded private key.
        /// </summary>
        /// <param name="privatePem">PEM-formatted private key string.</param>
        /// <returns>An <see cref="RsaSecurityKey"/> usable for JWT signing operations.</returns>
        /// <remarks>
        /// The key can be used to configure token signing credentials, typically in
        /// <see cref="Microsoft.IdentityModel.Tokens.SigningCredentials"/>.
        /// </remarks>
        public static RsaSecurityKey CreateSecurityKeyFromPrivatePem(string privatePem)
        {
            var rsa = LoadPrivateKeyFromPem(privatePem);
            return new RsaSecurityKey(rsa);
        }

        /// <summary>
        /// Creates a <see cref="RsaSecurityKey"/> instance from a PEM-encoded public key.
        /// </summary>
        /// <param name="publicPem">PEM-formatted public key string.</param>
        /// <returns>An <see cref="RsaSecurityKey"/> usable for JWT signature verification.</returns>
        /// <remarks>
        /// This key type is typically used by consumers or downstream services to validate
        /// tokens signed with the corresponding private key.
        /// </remarks>
        public static RsaSecurityKey CreateSecurityKeyFromPublicPem(string publicPem)
        {
            var rsa = LoadPublicKeyFromPem(publicPem);
            return new RsaSecurityKey(rsa);
        }
    }
}

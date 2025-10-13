using System.Security.Cryptography;
using System.Text;

namespace KeyGenerator
{
    public static class RsaKeyUtils
    {
        public static (string privatePem, string publicPem) GenerateRsaKeyPair(int keySize = 3072)
        {
            using var rsa = RSA.Create(keySize);

            var privateKeyBytes = rsa.ExportPkcs8PrivateKey();
            var privatePem = PemEncode("PRIVATE KEY", privateKeyBytes);

            var publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();
            var publicPem = PemEncode("PUBLIC KEY", publicKeyBytes);

            return (privatePem, publicPem);
        }

        private static string PemEncode(string label, byte[] data)
        {
            const int lineLength = 64;
            var base64 = Convert.ToBase64String(data);
            var sb = new StringBuilder();
            sb.AppendLine($"-----BEGIN {label}-----");
            for (int i = 0; i < base64.Length; i += lineLength)
                sb.AppendLine(base64.Substring(i, Math.Min(lineLength, base64.Length - i)));
            sb.AppendLine($"-----END {label}-----");
            return sb.ToString();
        }
    }

}

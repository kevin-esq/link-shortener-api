using KeyGenerator;
using System;
using System.IO;

class Program
{
    static void Main()
    {
        var (privatePem, publicPem) = RsaKeyUtils.GenerateRsaKeyPair();

        Directory.CreateDirectory("Keys");

        File.WriteAllText("Keys/private.pem", privatePem);
        File.WriteAllText("Keys/public.pem", publicPem);

        Console.WriteLine("🔐 RSA key pair generated successfully!");
        Console.WriteLine("📁 Keys saved to: ./Keys/private.pem & ./Keys/public.pem");
    }
}

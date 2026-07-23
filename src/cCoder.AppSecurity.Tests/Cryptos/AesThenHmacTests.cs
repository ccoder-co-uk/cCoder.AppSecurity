// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;


namespace cCoder.Core.Services.Tests.Cryptos;

public partial class AesThenHmacTests
{
    private static AesThenHmac CreateSut() => new();

    private static byte[] CreatePlainTextBytes() =>
        Encoding.UTF8.GetBytes("legacy message payload");

    private static string CreatePassword() => "StrongPassword123!";

    private static byte[] CreateNonSecretPayload() => Encoding.UTF8.GetBytes("prefix");

    private static byte[] CreateLegacyEncryptedMessage(
        AesThenHmac aesThenHmac,
        byte[] plainText,
        string password,
        byte[] nonSecretPayload
    )
    {
        byte[] cryptSalt = [1, 2, 3, 4, 5, 6, 7, 8];
        byte[] authSalt = [11, 12, 13, 14, 15, 16, 17, 18];
        byte[] payload = new byte[nonSecretPayload.Length + cryptSalt.Length + authSalt.Length];

        Array.Copy(nonSecretPayload, payload, nonSecretPayload.Length);
        Array.Copy(cryptSalt, 0, payload, nonSecretPayload.Length, cryptSalt.Length);
        Array.Copy(
            authSalt,
            0,
            payload,
            nonSecretPayload.Length + cryptSalt.Length,
            authSalt.Length
        );

#pragma warning disable SYSLIB0060
        using Rfc2898DeriveBytes cryptGenerator = new(
            password,
            cryptSalt,
            aesThenHmac.Iterations,
            HashAlgorithmName.SHA1
        );
        using Rfc2898DeriveBytes authGenerator = new(
            password,
            authSalt,
            aesThenHmac.Iterations,
            HashAlgorithmName.SHA1
        );
#pragma warning restore SYSLIB0060

        byte[] cryptKey = cryptGenerator.GetBytes(aesThenHmac.KeyByteSize);
        byte[] authKey = authGenerator.GetBytes(aesThenHmac.KeyByteSize);

        return aesThenHmac.SimpleEncrypt(plainText, cryptKey, authKey, payload);
    }
}
// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;

namespace cCoder.Core.Services.Tests.Cryptos;

internal class AesThenHmac
{
    private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();

    public int BlockBitSize { get; set; } = 128;
    public int KeyBitSize { get; set; } = 256;
    public int SaltBitSize { get; set; } = 64;
    public int Iterations { get; set; } = 10000;
    public int MinPasswordLength { get; set; } = 12;

    public int KeyByteSize => KeyBitSize / 8;
    public int SaltByteSize => SaltBitSize / 8;

    public byte[] NewKey()
    {
        byte[] key = new byte[KeyBitSize / 8];
        Random.GetBytes(key);
        return key;
    }

    public string SimpleEncrypt(string secretMessage, byte[] cryptKey, byte[] authKey, byte[] nonSecretPayload = null)
    {
        if (string.IsNullOrEmpty(secretMessage))
            throw new ArgumentException("Secret Message Required!", nameof(secretMessage));

        byte[] plainText = Encoding.UTF8.GetBytes(secretMessage);
        byte[] cipherText = SimpleEncrypt(plainText, cryptKey, authKey, nonSecretPayload);
        return Convert.ToBase64String(cipherText);
    }

    public byte[] SimpleEncrypt(byte[] secretMessage, byte[] cryptKey, byte[] authKey, byte[] nonSecretPayload = null)
    {
        if (cryptKey == null || cryptKey.Length != KeyBitSize / 8)
            throw new ArgumentException(string.Format("Key needs to be {0} bit!", KeyBitSize), nameof(cryptKey));

        if (authKey == null || authKey.Length != KeyBitSize / 8)
            throw new ArgumentException(string.Format("Key needs to be {0} bit!", KeyBitSize), nameof(authKey));

        if (secretMessage == null || secretMessage.Length < 1)
            throw new ArgumentException("Secret Message Required!", nameof(secretMessage));

        nonSecretPayload ??= Array.Empty<byte>();

        byte[] cipherText;
        byte[] iv;

        using Aes aes = Aes.Create();
        aes.BlockSize = BlockBitSize;
        aes.KeySize = KeyBitSize;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        aes.GenerateIV();
        iv = aes.IV;

        using (ICryptoTransform encrypter = aes.CreateEncryptor(cryptKey, iv))
        using (MemoryStream cipherStream = new())
        {
            using (CryptoStream cryptoStream = new(cipherStream, encrypter, CryptoStreamMode.Write))
            using (BinaryWriter binaryWriter = new(cryptoStream))
            {
                binaryWriter.Write(secretMessage);
            }

            cipherText = cipherStream.ToArray();
        }

        using HMACSHA256 hmac = new(authKey);
        using MemoryStream encryptedStream = new();
        using (BinaryWriter binaryWriter = new(encryptedStream))
        {
            binaryWriter.Write(nonSecretPayload);
            binaryWriter.Write(iv);
            binaryWriter.Write(cipherText);
            binaryWriter.Flush();

            byte[] tag = hmac.ComputeHash(encryptedStream.ToArray());
            binaryWriter.Write(tag);
        }

        return encryptedStream.ToArray();
    }

    public string SimpleDecrypt(string encryptedMessage, byte[] cryptKey, byte[] authKey, int nonSecretPayloadLength = 0)
    {
        if (string.IsNullOrWhiteSpace(encryptedMessage))
            throw new ArgumentException("Encrypted Message Required!", nameof(encryptedMessage));

        byte[] cipherText = Convert.FromBase64String(encryptedMessage);
        byte[] plainText = SimpleDecrypt(cipherText, cryptKey, authKey, nonSecretPayloadLength);
        return plainText == null ? null : Encoding.UTF8.GetString(plainText);
    }

    public byte[] SimpleDecrypt(byte[] encryptedMessage, byte[] cryptKey, byte[] authKey, int nonSecretPayloadLength = 0)
    {
        if (cryptKey == null || cryptKey.Length != KeyByteSize)
            throw new ArgumentException(string.Format("CryptKey needs to be {0} bit!", KeyBitSize), nameof(cryptKey));

        if (authKey == null || authKey.Length != KeyByteSize)
            throw new ArgumentException(string.Format("AuthKey needs to be {0} bit!", KeyBitSize), nameof(authKey));

        if (encryptedMessage == null || encryptedMessage.Length == 0)
            throw new ArgumentException("Encrypted Message Required!", nameof(encryptedMessage));

        using HMACSHA256 hmac = new(authKey);
        byte[] sentTag = new byte[hmac.HashSize / 8];
        byte[] calcTag = hmac.ComputeHash(encryptedMessage, 0, encryptedMessage.Length - sentTag.Length);
        int ivLength = BlockBitSize / 8;

        if (encryptedMessage.Length < sentTag.Length + nonSecretPayloadLength + ivLength)
            return Array.Empty<byte>();

        Array.Copy(encryptedMessage, encryptedMessage.Length - sentTag.Length, sentTag, 0, sentTag.Length);

        int compare = 0;
        for (int i = 0; i < sentTag.Length; i++)
            compare |= sentTag[i] ^ calcTag[i];

        if (compare != 0)
            return Array.Empty<byte>();

        using Aes aes = Aes.Create();
        aes.BlockSize = BlockBitSize;
        aes.KeySize = KeyBitSize;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        byte[] iv = new byte[ivLength];
        Array.Copy(encryptedMessage, nonSecretPayloadLength, iv, 0, iv.Length);

        using ICryptoTransform decrypter = aes.CreateDecryptor(cryptKey, iv);
        using MemoryStream plainTextStream = new();
        using (CryptoStream decrypterStream = new(plainTextStream, decrypter, CryptoStreamMode.Write))
        using (BinaryWriter binaryWriter = new(decrypterStream))
        {
            binaryWriter.Write(
                encryptedMessage,
                nonSecretPayloadLength + iv.Length,
                encryptedMessage.Length - nonSecretPayloadLength - iv.Length - sentTag.Length);
        }

        return plainTextStream.ToArray();
    }

    public string SimpleEncryptWithPassword(string secretMessage, string password, byte[] nonSecretPayload = null)
    {
        if (string.IsNullOrEmpty(secretMessage))
            throw new ArgumentException("Secret Message Required!", nameof(secretMessage));

        byte[] plainText = Encoding.UTF8.GetBytes(secretMessage);
        byte[] cipherText = SimpleEncryptWithPassword(plainText, password, nonSecretPayload);
        return Convert.ToBase64String(cipherText);
    }

    public byte[] SimpleEncryptWithPassword(byte[] secretMessage, string password, byte[] nonSecretPayload = null)
    {
        nonSecretPayload ??= Array.Empty<byte>();

        if (string.IsNullOrWhiteSpace(password) || password.Length < MinPasswordLength)
            throw new ArgumentException(string.Format("Must have a password of at least {0} characters!", MinPasswordLength), nameof(password));

        if (secretMessage == null || secretMessage.Length == 0)
            throw new ArgumentException("Secret Message Required!", nameof(secretMessage));

        byte[] payload = new byte[(SaltBitSize / 8 * 2) + nonSecretPayload.Length];
        Array.Copy(nonSecretPayload, payload, nonSecretPayload.Length);
        int payloadIndex = nonSecretPayload.Length;

        byte[] cryptSalt = new byte[SaltBitSize / 8];
        Random.GetBytes(cryptSalt);
        byte[] cryptKey = Rfc2898DeriveBytes.Pbkdf2(password, cryptSalt, Iterations, HashAlgorithmName.SHA1, KeyBitSize / 8);
        Array.Copy(cryptSalt, 0, payload, payloadIndex, cryptSalt.Length);
        payloadIndex += cryptSalt.Length;

        byte[] authSalt = new byte[SaltBitSize / 8];
        Random.GetBytes(authSalt);
        byte[] authKey = Rfc2898DeriveBytes.Pbkdf2(password, authSalt, Iterations, HashAlgorithmName.SHA1, KeyBitSize / 8);
        Array.Copy(authSalt, 0, payload, payloadIndex, authSalt.Length);

        return SimpleEncrypt(secretMessage, cryptKey, authKey, payload);
    }

    public string SimpleDecryptWithPassword(string encryptedMessage, string password, int nonSecretPayloadLength = 0)
    {
        if (string.IsNullOrWhiteSpace(encryptedMessage))
            throw new ArgumentException("Encrypted Message Required!", nameof(encryptedMessage));

        byte[] cipherText = Convert.FromBase64String(encryptedMessage);
        byte[] plainText = SimpleDecryptWithPassword(cipherText, password, nonSecretPayloadLength);
        return plainText == null ? null : Encoding.UTF8.GetString(plainText);
    }

    public byte[] SimpleDecryptWithPassword(byte[] encryptedMessage, string password, int nonSecretPayloadLength = 0)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < MinPasswordLength)
            throw new ArgumentException(string.Format("Must have a password of at least {0} characters!", MinPasswordLength), nameof(password));

        if (encryptedMessage == null || encryptedMessage.Length == 0)
            throw new ArgumentException("Encrypted Message Required!", nameof(encryptedMessage));

        byte[] cryptSalt = encryptedMessage.Skip(nonSecretPayloadLength).Take(SaltByteSize).ToArray();
        byte[] authSalt = encryptedMessage.Skip(nonSecretPayloadLength + cryptSalt.Length).Take(SaltByteSize).ToArray();
        byte[] cryptKey = Rfc2898DeriveBytes.Pbkdf2(password, cryptSalt, Iterations, HashAlgorithmName.SHA1, KeyByteSize);
        byte[] authKey = Rfc2898DeriveBytes.Pbkdf2(password, authSalt, Iterations, HashAlgorithmName.SHA1, KeyByteSize);

        return SimpleDecrypt(encryptedMessage, cryptKey, authKey, cryptSalt.Length + authSalt.Length + nonSecretPayloadLength);
    }
}
using FluentAssertions;
using Xunit;


namespace cCoder.Core.Services.Tests.Cryptos;

public partial class AesThenHmacTests
{
    [Fact]
    public void ShouldDecryptMessageWhenCipherWasCreatedWithLegacyDerivationForSimpleDecryptWithPassword()
    {
        // Given
        AesThenHmac aesThenHmac = CreateSut();
        byte[] plainText = CreatePlainTextBytes();
        string password = CreatePassword();
        byte[] nonSecretPayload = CreateNonSecretPayload();
        byte[] encryptedMessage = CreateLegacyEncryptedMessage(
            aesThenHmac,
            plainText,
            password,
            nonSecretPayload
        );

        // When
        byte[] decryptedMessage = aesThenHmac.SimpleDecryptWithPassword(
            encryptedMessage,
            password,
            nonSecretPayload.Length
        );

        // Then
        decryptedMessage.Should().Equal(plainText);
    }

}





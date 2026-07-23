// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using FluentAssertions;
using Xunit;


namespace cCoder.Core.Services.Tests.Cryptos;

public partial class AesThenHmacTests
{
    [Fact]
    public void ShouldReturnOriginalMessageWhenRoundTrippedForSimpleEncryptWithPassword()
    {
        // Given
        AesThenHmac aesThenHmac = CreateSut();
        byte[] plainText = CreatePlainTextBytes();
        string password = CreatePassword();
        byte[] nonSecretPayload = CreateNonSecretPayload();

        // When
        byte[] encryptedMessage = aesThenHmac.SimpleEncryptWithPassword(
            plainText,
            password,
            nonSecretPayload
        );
        byte[] decryptedMessage = aesThenHmac.SimpleDecryptWithPassword(
            encryptedMessage,
            password,
            nonSecretPayload.Length
        );

        // Then
        decryptedMessage.Should().Equal(plainText);
        Encoding.UTF8.GetString(decryptedMessage).Should().Be("legacy message payload");
    }

}
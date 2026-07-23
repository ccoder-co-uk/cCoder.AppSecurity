// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class RoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseFoundationReadsAndAddAsyncForNewItemWhenAddOrUpdate()
    {
        // Given
        Role entity = CreateRandomRole();
        entity.Id = Guid.Empty;
        roleServiceMock.Setup(x => x.AddAsync(entity)).ReturnsAsync(entity);

        // When
        Result<Role>[] results = (
            await roleProcessingService.AddOrUpdate(new[] { entity })
        ).ToArray();

        // Then
        results.Should().ContainSingle();
        results[0].Success.Should().BeTrue();
        results[0].Item.Should().BeSameAs(entity);
        results[0].Message.Should().Be("Added Successfully");
        roleServiceMock.Verify(x => x.AddAsync(entity), Times.Once);
        roleServiceMock.VerifyNoOtherCalls();
    }

}
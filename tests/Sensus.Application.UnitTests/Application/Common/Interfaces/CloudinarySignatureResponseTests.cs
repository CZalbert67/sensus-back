using FluentAssertions;
using Sensus.Application.Common.Interfaces;
using Xunit;

namespace Sensus.Application.UnitTests.Application.Common.Interfaces;

public class CloudinarySignatureResponseTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var response = new CloudinarySignatureResponse(
            "cloud-name",
            "api-key-123",
            1700000000L,
            "profile-images",
            "signature-abc"
        );

        response.CloudName.Should().Be("cloud-name");
        response.ApiKey.Should().Be("api-key-123");
        response.Timestamp.Should().Be(1700000000L);
        response.Folder.Should().Be("profile-images");
        response.Signature.Should().Be("signature-abc");
    }
}

using FluentAssertions;
using Sensus.Application.Users.DTOs;
using Xunit;

namespace Sensus.Application.UnitTests.Application.Users.DTOs;

public class GuestSessionResponseDtoTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var dto = new GuestSessionResponseDto(
            "token-123",
            "Bearer",
            DateTime.UtcNow.AddHours(1),
            2000,
            "Invitado"
        );

        dto.Token.Should().Be("token-123");
        dto.TokenType.Should().Be("Bearer");
        dto.MaxRadiusMeters.Should().Be(2000);
        dto.Role.Should().Be("Invitado");
        dto.ExpiresAtUtc.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void Record_ShouldSupportEquality()
    {
        var now = DateTime.UtcNow.AddHours(1);
        var a = new GuestSessionResponseDto("token", "Bearer", now, 2000, "Invitado");
        var b = new GuestSessionResponseDto("token", "Bearer", now, 2000, "Invitado");

        a.Should().Be(b);
    }
}

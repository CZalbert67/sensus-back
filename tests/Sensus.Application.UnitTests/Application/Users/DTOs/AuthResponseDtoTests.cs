using FluentAssertions;
using Sensus.Application.Users.DTOs;
using Xunit;

namespace Sensus.Application.UnitTests.Application.Users.DTOs;

public class AuthResponseDtoTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var userDto = new UserDto(
            Guid.NewGuid(),
            "Juan Pérez",
            "juan@sensus.com",
            new List<string> { "Consumidor" },
            true,
            DateTime.UtcNow.AddDays(-30),
            DateTime.UtcNow,
            "https://cloudinary.com/foto.jpg"
        );

        var dto = new AuthResponseDto("token-abc", "Bearer", DateTime.UtcNow.AddHours(1), userDto);

        dto.Token.Should().Be("token-abc");
        dto.TokenType.Should().Be("Bearer");
        dto.User.Should().BeSameAs(userDto);
        dto.ExpiresAtUtc.Should().BeCloseTo(DateTime.UtcNow.AddHours(1), TimeSpan.FromMinutes(1));
    }
}

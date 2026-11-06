using FluentAssertions;
using Sensus.Application.Users.DTOs;
using Xunit;

namespace Sensus.Application.UnitTests.Application.Users.DTOs;

public class UserDtoTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var now = DateTime.UtcNow;
        var dto = new UserDto(
            Guid.NewGuid(),
            "Juan Pérez",
            "juan@sensus.com",
            new List<string> { "Consumidor", "Comerciante" },
            true,
            now.AddDays(-30),
            now,
            "https://cloudinary.com/foto.jpg"
        );

        dto.NombreCompleto.Should().Be("Juan Pérez");
        dto.Correo.Should().Be("juan@sensus.com");
        dto.Roles.Should().HaveCount(2);
        dto.Estatus.Should().BeTrue();
        dto.FechaRegistro.Should().BeCloseTo(now.AddDays(-30), TimeSpan.FromMinutes(1));
        dto.UltimoAcceso.Should().BeCloseTo(now, TimeSpan.FromMinutes(1));
        dto.FotoPerfilUrl.Should().Be("https://cloudinary.com/foto.jpg");
    }

    [Fact]
    public void Constructor_WithoutFotoPerfilUrl_ShouldDefaultToNull()
    {
        var dto = new UserDto(
            Guid.NewGuid(),
            "Juan",
            "juan@sensus.com",
            new List<string> { "Invitado" },
            false,
            DateTime.UtcNow,
            null,
            null
        );

        dto.FotoPerfilUrl.Should().BeNull();
    }
}

using FluentAssertions;
using Sensus.Domain.Enums;
using Xunit;

namespace Sensus.Application.UnitTests.Domain.Enums;

public class RoleTypeTests
{
    [Fact]
    public void Constants_ShouldHaveExpectedValues()
    {
        RoleType.Consumidor.Should().Be("Consumidor");
        RoleType.Comerciante.Should().Be("Comerciante");
        RoleType.Administrador.Should().Be("Administrador");
        RoleType.Invitado.Should().Be("Invitado");
    }

    [Fact]
    public void All_ShouldContainAllRoles()
    {
        RoleType.All.Should().HaveCount(4);
        RoleType.All.Should().Contain(RoleType.Consumidor);
        RoleType.All.Should().Contain(RoleType.Comerciante);
        RoleType.All.Should().Contain(RoleType.Administrador);
        RoleType.All.Should().Contain(RoleType.Invitado);
    }
}

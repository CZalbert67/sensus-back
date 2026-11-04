using FluentAssertions;
using Sensus.Application.Common.Exceptions;
using Xunit;

namespace Sensus.Application.UnitTests.Application.Common.Exceptions;

public class NotFoundExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetMessageWithNameAndKey()
    {
        var ex = new NotFoundException("Usuario", Guid.NewGuid());

        ex.Message.Should().Contain("Usuario");
        ex.Message.Should().Contain("no fue encontrada");
    }
}

using FluentAssertions;
using Sensus.Domain.Enums;
using Sensus.Domain.Exceptions;
using Xunit;

namespace Sensus.Application.UnitTests.Domain.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        var message = "Error de dominio";
        var ex = new DomainException(message);

        ex.Message.Should().Be(message);
        ex.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldSetBoth()
    {
        var message = "Error de dominio";
        var inner = new InvalidOperationException("error interno");
        var ex = new DomainException(message, inner);

        ex.Message.Should().Be(message);
        ex.InnerException.Should().BeSameAs(inner);
    }
}

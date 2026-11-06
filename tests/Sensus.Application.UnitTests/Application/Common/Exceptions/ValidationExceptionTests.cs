using FluentAssertions;
using FluentValidation.Results;
using Sensus.Application.Common.Exceptions;
using Xunit;

namespace Sensus.Application.UnitTests.Application.Common.Exceptions;

public class ValidationExceptionTests
{
    [Fact]
    public void DefaultConstructor_ShouldInitializeEmptyErrors()
    {
        var ex = new ValidationException();

        ex.Message.Should().Be("Se produjeron uno o más errores de validación.");
        ex.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ConstructorWithFailures_ShouldGroupErrorsByProperty()
    {
        var failures = new[]
        {
            new ValidationFailure("Email", "El email es obligatorio."),
            new ValidationFailure("Email", "El email no es válido."),
            new ValidationFailure("Nombre", "El nombre es obligatorio.")
        };

        var ex = new ValidationException(failures);

        ex.Errors.Should().HaveCount(2);
        ex.Errors["Email"].Should().HaveCount(2);
        ex.Errors["Nombre"].Should().HaveCount(1);
        ex.Errors["Nombre"][0].Should().Be("El nombre es obligatorio.");
    }
}

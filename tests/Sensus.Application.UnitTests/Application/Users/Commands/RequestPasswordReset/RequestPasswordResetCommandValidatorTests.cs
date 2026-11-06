using FluentAssertions;
using Sensus.Application.Users.Commands.RequestPasswordReset;
using Xunit;

namespace Sensus.Application.UnitTests.Application.Users.Commands.RequestPasswordReset;

public class RequestPasswordResetCommandValidatorTests
{
    private readonly RequestPasswordResetCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsEmpty()
    {
        var result = _validator.Validate(new RequestPasswordResetCommand(""));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Correo");
    }

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsInvalid()
    {
        var result = _validator.Validate(new RequestPasswordResetCommand("not-an-email"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Correo");
    }

    [Fact]
    public void Validate_ShouldPass_WhenEmailIsValid()
    {
        var result = _validator.Validate(new RequestPasswordResetCommand("user@sensus.com"));

        result.IsValid.Should().BeTrue();
    }
}

using FluentAssertions;
using Sensus.Application.Users.Commands.ResetPassword;
using Xunit;

namespace Sensus.Application.UnitTests.Application.Users.Commands.ResetPassword;

public class ResetPasswordCommandValidatorTests
{
    private readonly ResetPasswordCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldFail_WhenTokenIsEmpty()
    {
        var result = _validator.Validate(new ResetPasswordCommand("", "Abcdef12"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Token");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordIsTooShort()
    {
        var result = _validator.Validate(new ResetPasswordCommand("token", "Abc1"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NuevaPassword");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordMissingUppercase()
    {
        var result = _validator.Validate(new ResetPasswordCommand("token", "ABCDEF12"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NuevaPassword");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordMissingLowercase()
    {
        var result = _validator.Validate(new ResetPasswordCommand("token", "abcdef12"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NuevaPassword");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordMissingDigit()
    {
        var result = _validator.Validate(new ResetPasswordCommand("token", "Abcdefgh"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NuevaPassword");
    }

    [Fact]
    public void Validate_ShouldPass_WhenValid()
    {
        var result = _validator.Validate(new ResetPasswordCommand("token", "Abcdef12"));

        result.IsValid.Should().BeTrue();
    }
}

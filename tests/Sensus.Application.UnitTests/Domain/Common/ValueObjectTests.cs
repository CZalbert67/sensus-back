using FluentAssertions;
using Sensus.Domain.Common;
using Sensus.Domain.ValueObjects;
using Xunit;

namespace Sensus.Application.UnitTests.Domain.Common;

public class ValueObjectTests
{
    private sealed class TestValueObject : ValueObject
    {
        public string Name { get; }
        public int Value { get; }

        public TestValueObject(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Name;
            yield return Value;
        }
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenSameValues()
    {
        var a = new TestValueObject("test", 1);
        var b = new TestValueObject("test", 1);

        a.Equals(b).Should().BeTrue();
        (a == b).Should().BeTrue();
        (a != b).Should().BeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenDifferentValues()
    {
        var a = new TestValueObject("a", 1);
        var b = new TestValueObject("b", 2);

        a.Equals(b).Should().BeFalse();
        (a == b).Should().BeFalse();
        (a != b).Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenDifferentTypes()
    {
        var horario = HorarioDia.Cerrado(DayOfWeek.Monday).Value;
        var other = new TestValueObject("x", 1);

        horario.Equals(other).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_ShouldBeEqual_WhenSameValues()
    {
        var a = new TestValueObject("test", 1);
        var b = new TestValueObject("test", 1);

        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void HorarioDia_Equals_ShouldWork_WhenSameValues()
    {
        var a = HorarioDia.Abierto(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0)).Value;
        var b = HorarioDia.Abierto(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0)).Value;

        a.Equals(b).Should().BeTrue();
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void HorarioDia_EsAbiertoEn_ShouldReturnTrue_WithinHours()
    {
        var horario = HorarioDia.Abierto(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0)).Value;

        horario.EsAbiertoEn(DayOfWeek.Monday, new TimeOnly(12, 0)).Should().BeTrue();
    }

    [Fact]
    public void HorarioDia_EsAbiertoEn_ShouldReturnFalse_OutsideHours()
    {
        var horario = HorarioDia.Abierto(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0)).Value;

        horario.EsAbiertoEn(DayOfWeek.Monday, new TimeOnly(8, 0)).Should().BeFalse();
        horario.EsAbiertoEn(DayOfWeek.Monday, new TimeOnly(17, 0)).Should().BeFalse();
    }

    [Fact]
    public void HorarioDia_EsAbiertoEn_ShouldReturnFalse_WhenClosed()
    {
        var horario = HorarioDia.Cerrado(DayOfWeek.Monday).Value;

        horario.EsAbiertoEn(DayOfWeek.Monday, new TimeOnly(12, 0)).Should().BeFalse();
    }

    [Fact]
    public void HorarioDia_EsAbiertoEn_ShouldReturnFalse_WrongDay()
    {
        var horario = HorarioDia.Abierto(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0)).Value;

        horario.EsAbiertoEn(DayOfWeek.Tuesday, new TimeOnly(12, 0)).Should().BeFalse();
    }
}

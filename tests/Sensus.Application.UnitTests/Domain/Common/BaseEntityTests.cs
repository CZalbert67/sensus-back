using FluentAssertions;
using Sensus.Domain.Common;
using Sensus.Domain.Entities;
using Xunit;

namespace Sensus.Application.UnitTests.Domain.Common;

public class BaseEntityTests
{
    private sealed class TestEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public TestEntity(string name) : base()
        {
            Name = name;
        }

        public TestEntity(string name, Guid id) : base(id)
        {
            Name = name;
        }
    }

    [Fact]
    public void Constructor_ShouldGenerateId()
    {
        var entity = new TestEntity("test");

        entity.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Constructor_WithId_ShouldSetId()
    {
        var id = Guid.NewGuid();
        var entity = new TestEntity("test", id);

        entity.Id.Should().Be(id);
    }

    [Fact]
    public void AddDomainEvent_ShouldAddEvent()
    {
        var entity = new TestEntity("test");
        var domainEvent = new ComercioRegisteredDomainEventStub();

        entity.AddDomainEvent(domainEvent);

        entity.DomainEvents.Should().HaveCount(1);
        entity.DomainEvents.First().Should().Be(domainEvent);
    }

    [Fact]
    public void RemoveDomainEvent_ShouldRemoveEvent()
    {
        var entity = new TestEntity("test");
        var domainEvent = new ComercioRegisteredDomainEventStub();

        entity.AddDomainEvent(domainEvent);
        entity.RemoveDomainEvent(domainEvent);

        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ClearDomainEvents_ShouldClearAllEvents()
    {
        var entity = new TestEntity("test");
        entity.AddDomainEvent(new ComercioRegisteredDomainEventStub());
        entity.AddDomainEvent(new ComercioRegisteredDomainEventStub());

        entity.ClearDomainEvents();

        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenSameIdAndType()
    {
        var id = Guid.NewGuid();
        var a = new TestEntity("a", id);
        var b = new TestEntity("b", id);

        a.Equals(b).Should().BeTrue();
        (a == b).Should().BeTrue();
        (a != b).Should().BeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenDifferentId()
    {
        var a = new TestEntity("a", Guid.NewGuid());
        var b = new TestEntity("b", Guid.NewGuid());

        a.Equals(b).Should().BeFalse();
        (a == b).Should().BeFalse();
        (a != b).Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenNull()
    {
        var entity = new TestEntity("test");

        entity.Equals(null).Should().BeFalse();
        (entity == null).Should().BeFalse();
        (null == entity).Should().BeFalse();
    }

    private sealed class ComercioRegisteredDomainEventStub : IDomainEvent
    {
        public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
    }
}

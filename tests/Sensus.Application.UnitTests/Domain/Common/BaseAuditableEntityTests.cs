using FluentAssertions;
using Sensus.Domain.Common;
using Xunit;

namespace Sensus.Application.UnitTests.Domain.Common;

public class BaseAuditableEntityTests
{
    private sealed class TestAuditableEntity : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;

        public TestAuditableEntity(string name)
        {
            Name = name;
        }

        public TestAuditableEntity(string name, Guid id) : base(id)
        {
            Name = name;
        }
    }

    [Fact]
    public void Constructor_ShouldGenerateId()
    {
        var entity = new TestAuditableEntity("test");

        entity.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Constructor_WithId_ShouldSetId()
    {
        var id = Guid.NewGuid();
        var entity = new TestAuditableEntity("test", id);

        entity.Id.Should().Be(id);
    }

    [Fact]
    public void AuditableProperties_ShouldBeSettable()
    {
        var now = DateTime.UtcNow;
        var entity = new TestAuditableEntity("test")
        {
            CreatedAtUtc = now,
            CreatedBy = "user1",
            LastModifiedAtUtc = now,
            LastModifiedBy = "user2"
        };

        entity.CreatedAtUtc.Should().Be(now);
        entity.CreatedBy.Should().Be("user1");
        entity.LastModifiedAtUtc.Should().Be(now);
        entity.LastModifiedBy.Should().Be("user2");
    }
}

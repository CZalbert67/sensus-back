using FluentAssertions;
using Sensus.Application.Common.Models;
using Xunit;

namespace Sensus.Application.UnitTests.Application.Common.Models;

public class PagedResultTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var items = new List<int> { 1, 2, 3 };
        var result = new PagedResult<int>(items, 100, 1, 10);

        result.Items.Should().BeEquivalentTo(items);
        result.TotalCount.Should().Be(100);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Theory]
    [InlineData(100, 1, 10, 10)]
    [InlineData(100, 2, 10, 10)]
    [InlineData(105, 1, 10, 11)]
    [InlineData(0, 1, 10, 0)]
    public void TotalPages_ShouldBeCorrect(int totalCount, int pageNumber, int pageSize, int expected)
    {
        var result = new PagedResult<int>(new List<int>(), totalCount, pageNumber, pageSize);

        result.TotalPages.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 100, 10, false, true)]
    [InlineData(2, 100, 10, true, true)]
    [InlineData(10, 100, 10, true, false)]
    [InlineData(1, 10, 10, false, false)]
    public void NavigationProperties_ShouldBeCorrect(int pageNumber, int totalCount, int pageSize, bool hasPrevious, bool hasNext)
    {
        var result = new PagedResult<int>(new List<int>(), totalCount, pageNumber, pageSize);

        result.HasPreviousPage.Should().Be(hasPrevious);
        result.HasNextPage.Should().Be(hasNext);
    }
}

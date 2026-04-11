using Clean.Architecture.Template.Application.Abstractions.Repositories;
using Clean.Architecture.Template.Application.Features.DummyItems;
using Clean.Architecture.Template.Domain;
using NSubstitute;

namespace Clean.Architecture.Template.Application.Tests.Features.DummyItems;

public class GetAllDummyItemsQueryHandlerTests
{
    private readonly IDummyItemRepository _repository = Substitute.For<IDummyItemRepository>();
    private readonly GetAllDummyItemsQueryHandler _sut;

    public GetAllDummyItemsQueryHandlerTests()
    {
        _sut = new GetAllDummyItemsQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WithAllItems()
    {
        var items = new List<DummyItem>
        {
            new() { Id = Guid.NewGuid(), Name = "Item 1" },
            new() { Id = Guid.NewGuid(), Name = "Item 2" }
        };
        _repository.ListAllAsync(Arg.Any<CancellationToken>()).Returns(items);

        var result = await _sut.Handle(new GetAllDummyItemsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task Handle_WhenNoItems_ShouldReturnEmptyList()
    {
        _repository.ListAllAsync(Arg.Any<CancellationToken>()).Returns(new List<DummyItem>());

        var result = await _sut.Handle(new GetAllDummyItemsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }
}

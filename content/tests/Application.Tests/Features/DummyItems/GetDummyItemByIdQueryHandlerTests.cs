using Clean.Architecture.Template.Application.Abstractions.Repositories;
using Clean.Architecture.Template.Application.Features.DummyItems;
using Clean.Architecture.Template.Domain;
using NSubstitute;

namespace Clean.Architecture.Template.Application.Tests.Features.DummyItems;

public class GetDummyItemByIdQueryHandlerTests
{
    private readonly IDummyItemRepository _repository = Substitute.For<IDummyItemRepository>();
    private readonly GetDummyItemByIdQueryHandler _sut;

    public GetDummyItemByIdQueryHandlerTests()
    {
        _sut = new GetDummyItemByIdQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenItemExists_ShouldReturnSuccess()
    {
        var id = Guid.NewGuid();
        var item = new DummyItem { Id = id, Name = "Found Item" };
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(item);

        var result = await _sut.Handle(new GetDummyItemByIdQuery(id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Found Item", result.Value.Name);
    }

    [Fact]
    public async Task Handle_WhenItemNotFound_ShouldReturnFailure()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((DummyItem?)null);

        var result = await _sut.Handle(new GetDummyItemByIdQuery(id), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(SharedKernel.ErrorCategory.NotFound, result.Error.ErrorCategory);
    }
}

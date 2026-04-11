using Clean.Architecture.Template.Application.Abstractions.Repositories;
using Clean.Architecture.Template.Application.Features.DummyItems;
using Clean.Architecture.Template.Domain;
using NSubstitute;

namespace Clean.Architecture.Template.Application.Tests.Features.DummyItems;

public class CreateDummyItemCommandHandlerTests
{
    private readonly IDummyItemRepository _repository = Substitute.For<IDummyItemRepository>();
    private readonly CreateDummyItemCommandHandler _sut;

    public CreateDummyItemCommandHandlerTests()
    {
        _repository.AddAsync(Arg.Any<DummyItem>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<DummyItem>());

        _sut = new CreateDummyItemCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WithCreatedEntity()
    {
        var command = new CreateDummyItemCommand("Test Item");

        var result = await _sut.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Test Item", result.Value.Name);
    }

    [Fact]
    public async Task Handle_ShouldCallRepository_AddAsync()
    {
        var command = new CreateDummyItemCommand("Test Item");

        await _sut.Handle(command, CancellationToken.None);

        await _repository.Received(1).AddAsync(
            Arg.Is<DummyItem>(e => e.Name == "Test Item"),
            Arg.Any<CancellationToken>());
    }
}

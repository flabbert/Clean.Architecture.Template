using Clean.Architecture.Template.Application.Abstractions.Repositories;
using Clean.Architecture.Template.Domain;
using Clean.Architecture.Template.SharedKernel;
using Mediator;

namespace Clean.Architecture.Template.Application.Features.DummyItems;

public sealed record CreateDummyItemCommand(string Name) : ICommand<Result<DummyItem>>;

public sealed class CreateDummyItemCommandHandler(IDummyItemRepository dummyItemRepository) : ICommandHandler<CreateDummyItemCommand, Result<DummyItem>>
{
    public async ValueTask<Result<DummyItem>> Handle(CreateDummyItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new DummyItem();
        entity.Name = request.Name;

        await dummyItemRepository.AddAsync(entity, cancellationToken);

        return entity;
    }
}
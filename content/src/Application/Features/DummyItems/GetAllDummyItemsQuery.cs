using Clean.Architecture.Template.Application.Abstractions.Repositories;
using Clean.Architecture.Template.Domain;
using Clean.Architecture.Template.SharedKernel;
using Mediator;

namespace Clean.Architecture.Template.Application.Features.DummyItems;

public sealed record GetAllDummyItemsQuery : IQuery<Result<List<DummyItemResponse>>>;

public sealed class GetAllDummyItemsQueryHandler(IDummyItemRepository dummyItemRepository)
    : IQueryHandler<GetAllDummyItemsQuery, Result<List<DummyItemResponse>>>
{
    public async ValueTask<Result<List<DummyItemResponse>>> Handle(
        GetAllDummyItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await dummyItemRepository.ListAllAsync(cancellationToken);
        return items.Select(i => new DummyItemResponse(i)).ToList();
    }
}

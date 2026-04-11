using Clean.Architecture.Template.Application.Abstractions.Repositories;
using Clean.Architecture.Template.Domain;
using Clean.Architecture.Template.SharedKernel;
using Mediator;

namespace Clean.Architecture.Template.Application.Features.DummyItems;

public sealed record GetDummyItemByIdQuery(Guid Id) : IQuery<Result<DummyItemResponse>>;

public sealed class GetDummyItemByIdQueryHandler(IDummyItemRepository dummyItemRepository)
    : IQueryHandler<GetDummyItemByIdQuery, Result<DummyItemResponse>>
{
    public async ValueTask<Result<DummyItemResponse>> Handle(
        GetDummyItemByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await dummyItemRepository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            return Result.Failure<DummyItemResponse>(DummyItemErrors.NotFound(request.Id));

        return new DummyItemResponse(entity);
    }
}

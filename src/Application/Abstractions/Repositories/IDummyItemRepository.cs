using Clean.Architecture.Template.Domain;

namespace Clean.Architecture.Template.Application.Abstractions.Repositories;

public interface IDummyItemRepository : IAsyncRepository<DummyItem,Guid>
{

}

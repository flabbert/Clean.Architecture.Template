using Clean.Architecture.Template.Application.Abstractions.Repositories;
using Clean.Architecture.Template.Domain;
using Clean.Architecture.Template.Infrastructure.Persistence;

namespace Clean.Architecture.Template.Infrastructure.Repositories;

public class DummyItemRepository(ApplicationDbContext dbContext)
    : BaseRepository<DummyItem, Guid>(dbContext), IDummyItemRepository;

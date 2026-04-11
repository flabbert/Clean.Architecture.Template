using Clean.Architecture.Template.Application.Abstractions.Repositories;
using Clean.Architecture.Template.Infrastructure.Persistence;
using Clean.Architecture.Template.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clean.Architecture.Template.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
#if (Postgres)
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection") ?? "Host=localhost;Database=appdb;Username=postgres;Password=postgres"));
#else
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.db"));
#endif

        services.AddScoped<IDummyItemRepository, DummyItemRepository>();

        return services;
    }
}

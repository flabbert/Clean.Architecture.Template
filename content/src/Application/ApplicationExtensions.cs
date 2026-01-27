using Clean.Architecture.Template.Application.Behaviours;
using Clean.Architecture.Template.Application.Features.DummyItems;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Clean.Architecture.Template.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddCoreApplication(this IServiceCollection services)
    {
        services.AddMediator((options) =>
            {
                options.ServiceLifetime = ServiceLifetime.Scoped;
                options.Assemblies = [Assembly.GetExecutingAssembly(), typeof(CreateDummyItemCommand).Assembly];
                options.PipelineBehaviors = [typeof(LoggingBehaviour<,>)];
                options.NotificationPublisherType = typeof(Mediator.ForeachAwaitPublisher);
            }
        );
        return services;
    }
}

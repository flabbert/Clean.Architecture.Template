namespace Clean.Architecture.Template.Api.ExceptionHandlers;

public static class ExceptionHandlerServiceExtensions
{
    public static IServiceCollection AddExceptionHandlers(this IServiceCollection services)
    {
        // order matters here, Global Exception handler should always be last
        services.AddExceptionHandler<NotImplementedExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        return services;
    }
}

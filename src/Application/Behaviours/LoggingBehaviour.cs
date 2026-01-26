using Clean.Architecture.Template.SharedKernel;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Clean.Architecture.Template.Application.Behaviours;
public sealed class LoggingBehaviour<TMessage, TResponse>(ILogger<LoggingBehaviour<TMessage, TResponse>> logger) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
    where TResponse : Result
{
    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("executing request: {messageType} with parameters: {@params}", message.GetType().Name, message);
            var response = await next(message, cancellationToken);
            if (!response.IsFailure) return response;
            
            if (response.Error.ErrorCategory == ErrorCategory.NotFound)
                logger.LogInformation("{messageType} - Request failed with: {@Error}", message.GetType().Name, response.Error);
            else
                logger.LogError("{messageType} - Request failed with: {@Error}", message.GetType().Name, response.Error);


            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{messageType} threw exception", message.GetType().Name);
            throw;
        }
    }
}
using Common.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(
        TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        _logger.LogInformation("----- Handling command {CommandName}. Request: {@Command}", 
            request.GetGenericTypeName(), request);
        var response = await next();
        _logger.LogInformation("----- Handled Command {CommandName}. Response: {@Response}", 
            request.GetGenericTypeName(), response);

        return response;
    }
}
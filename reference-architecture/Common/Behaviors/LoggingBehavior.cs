using EventDriven.CQRS.Abstractions.Behaviors;
using EventDriven.CQRS.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;   
    }
    
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, 
        RequestHandlerDelegate<TResponse> next)
    {
        string requestType = string.Empty;
        if (typeof(TRequest).IsCommandType())
            requestType = "command";
        else if (typeof(TRequest).IsQueryType())
            requestType = "query";
        _logger.LogInformation("----- Handling {RequestType} '{CommandName}'. Request: {@Request}", 
            requestType, request.GetGenericTypeName(), request);
        var response = await next();
        _logger.LogInformation("----- Handled {RequestType} '{CommandName}'. Response: {@Response}", 
            requestType, request.GetGenericTypeName(), response);
        return response;
    }
}
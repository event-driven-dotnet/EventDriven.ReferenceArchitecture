using MediatR;

namespace EventDriven.CQRS.Abstractions.Commands
{
    /// <summary>
    ///     Command Handler
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface ICommandHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : CommandResult { }
}
namespace EventDriven.CQRS.Abstractions.Commands {

    using MediatR;

    /// <summary>
    /// An object that is sent to the domain for a state change which is handled by a command handler.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface ICommand<out TResult> : IRequest<TResult> where TResult : CommandResult { }

}
using System.Collections.Generic;
using EventDriven.CQRS.Abstractions.Events;

namespace EventDriven.CQRS.Abstractions.Commands
{
    /// <summary>
    ///     Processes a command by generating one or more domain events.
    /// </summary>
    /// <typeparam name="TCommand">The type of command</typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface ICommandProcessor<in TCommand, TResult> where TCommand : class, ICommand<TResult>
                                                             where TResult : CommandResult
    {
        /// <summary>
        ///     Process specified command by creating one or more domain events.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <returns>Domain events resulting from the command.</returns>
        IEnumerable<IDomainEvent> Process(TCommand command);
    }
}
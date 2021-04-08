using EventDriven.CQRS.Abstractions.Events;
using System.Collections.Generic;

namespace EventDriven.CQRS.Abstractions.Commands
{
    /// <summary>
    /// Command handler.
    /// </summary>
    /// <typeparam name="TCommand">The type of command</typeparam>
    public interface ICommandProcessor<in TCommand> where TCommand : class, ICommand
    {
        /// <summary>
        /// Process specified command by creating a domain event.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <returns>Domain events to apply.</returns>
        IEnumerable<IDomainEvent> Process(TCommand command);
    }
}

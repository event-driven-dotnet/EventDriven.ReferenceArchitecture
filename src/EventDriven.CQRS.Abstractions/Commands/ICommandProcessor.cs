using EventDriven.CQRS.Abstractions.Events;
using System.Collections.Generic;

namespace EventDriven.CQRS.Abstractions.Commands
{
    /// <summary>
    /// Processes a command by generating one or more domain events.
    /// </summary>
    /// <typeparam name="TCommand">The type of command</typeparam>
    public interface ICommandProcessor<in TCommand> where TCommand : class, ICommand
    {
        /// <summary>
        /// Process specified command by creating one or more domain events.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <returns>Domain events resulting from the command.</returns>
        IEnumerable<IDomainEvent> Process(TCommand command);
    }
}

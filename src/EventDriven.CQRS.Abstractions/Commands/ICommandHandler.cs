using System.Threading.Tasks;
using EventDriven.CQRS.Abstractions.Entities;

namespace EventDriven.CQRS.Abstractions.Commands
{
    /// <summary>
    /// Command handler.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TCommand">The type of command.</typeparam>
    public interface ICommandHandler<TEntity, in TCommand>
        where TEntity : Entity
        where TCommand : class, ICommand
    {
        /// <summary>
        /// Handles a command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The command result.</returns>
        Task<CommandResult<TEntity>> Handle(TCommand command);
    }
}

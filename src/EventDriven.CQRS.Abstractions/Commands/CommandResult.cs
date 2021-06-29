using System.Collections.Generic;
using System.Linq;
using EventDriven.CQRS.Abstractions.Entities;

namespace EventDriven.CQRS.Abstractions.Commands
{
    /// <summary>
    ///     Represents the result of dispatching a command.
    /// </summary>
    public record CommandResult(CommandOutcome Outcome,
                                IDictionary<string, string[]> Errors = null);

    /// <summary>
    ///     Represents the result of dispatching a command.
    /// </summary>
    public record CommandResult<TEntity> : CommandResult
        where TEntity : Entity
    {
        /// <inheritdoc />
        public CommandResult(CommandOutcome outcome,
                             params TEntity[] entities) : base(outcome)
        {
            foreach (var entity in entities) Entities.Add(entity);
        }

        /// <inheritdoc />
        public CommandResult(CommandOutcome outcome,
                             IDictionary<string, string[]> errors,
                             params TEntity[] entities) : base(outcome, errors)
        {
            foreach (var entity in entities) Entities.Add(entity);
        }

        /// <summary>
        ///     Entities associated with the result.
        /// </summary>
        public List<TEntity> Entities { get; } = new();

        /// <summary>
        ///     Entity associated with the result.
        /// </summary>
        public TEntity Entity => Entities.FirstOrDefault();
    }
}
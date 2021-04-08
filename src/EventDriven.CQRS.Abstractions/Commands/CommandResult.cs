using System.Collections.Generic;
using System.Linq;
using EventDriven.CQRS.Abstractions.Entities;

namespace EventDriven.CQRS.Abstractions.Commands
{
    /// <summary>
    /// Represents the result of dispatching a command
    /// </summary>
    public record CommandResult(
        CommandOutcome Outcome,
        IDictionary<string, string[]> Errors = null);

    /// <summary>
    /// Represents the result of dispatching a command
    /// </summary>
    public record CommandResult<TEntity> : CommandResult
        where TEntity : Entity
    {
        public List<TEntity> Entities { get; } = new List<TEntity>();

        public TEntity Entity => Entities.FirstOrDefault();

        public CommandResult(
            CommandOutcome outcome,
            params TEntity[] entities) : base(outcome)
        {
            foreach (var entity in entities) Entities.Add(entity);
        }
        public CommandResult(
            CommandOutcome outcome,
            IDictionary<string, string[]> errors,
            params TEntity[] entities) : base(outcome, errors)
        {
            foreach (var entity in entities) Entities.Add(entity);
        }
    }
}

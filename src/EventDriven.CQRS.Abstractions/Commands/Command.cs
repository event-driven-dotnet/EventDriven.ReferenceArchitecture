using System;

namespace EventDriven.CQRS.Abstractions.Commands
{
    /// <summary>
    /// Represents a command.
    /// </summary>
    public record Command : ICommand
    {
        /// <inheritdoc />
        public Guid EntityId => default(Guid);

        /// <inheritdoc />
        public string EntityEtag { get; set; }

        /// <inheritdoc />
        public bool? EntityExists => false;

        /// <summary>
        /// Represents a Create command.
        /// </summary>
        /// <param name="EntityId">Represents the ID of the entity the command is in reference to.</param>
        public abstract record Create(
            Guid EntityId = default) : ICommand
        {
            /// <inheritdoc />
            public string EntityEtag { get; set; }

            /// <inheritdoc />
            public bool? EntityExists => false;
        }

        /// <summary>
        /// Represents an Update command.
        /// </summary>
        /// <param name="EntityId">Represents the ID of the entity the command is in reference to.</param>
        /// <param name="EntityEtag">If provided, refers to the version of the entity to update.</param>
        public abstract record Update(
            Guid EntityId,
            string EntityEtag = null) : ICommand
        {
            /// <inheritdoc />
            public string EntityEtag { get; set; } = EntityEtag;

            /// <inheritdoc />
            public bool? EntityExists => true;
        }

        /// <summary>
        /// Represents a Remove command.
        /// </summary>
        /// <param name="EntityId">Represents the ID of the entity the command is in reference to.</param>
        /// <param name="EntityEtag">If provided, refers to the version of the entity to update.</param>
        public abstract record Remove(
            Guid EntityId,
            string EntityEtag = null) : ICommand
        {
            /// <inheritdoc />
            public string EntityEtag { get; set; } = EntityEtag;

            /// <inheritdoc />
            public bool? EntityExists => true;
        }
    }
}

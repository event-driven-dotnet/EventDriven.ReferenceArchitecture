namespace EventDriven.CQRS.Abstractions.Commands
{
    /// <summary>
    ///     Outcome of a command.
    /// </summary>
    public enum CommandOutcome
    {
        /// <summary>
        ///     Only if the command was accepted
        /// </summary>
        Accepted,

        /// <summary>
        ///     If the command was rejected due to a conflict.
        /// </summary>
        Conflict,

        /// <summary>
        ///     If the command was invalid due to its parameters.
        /// </summary>
        InvalidCommand,

        /// <summary>
        ///     If the command was invalid due to the object state.
        /// </summary>
        InvalidState,

        /// <summary>
        ///     Not handled.
        /// </summary>
        NotHandled,

        /// <summary>
        ///     Entity was not found.
        /// </summary>
        NotFound
    }
}
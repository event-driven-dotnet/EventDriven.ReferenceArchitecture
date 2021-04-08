namespace EventDriven.CQRS.Abstractions.Commands
{
    public enum CommandOutcome
    {
        /// only if the command was accepted
        Accepted,
        /// if the command was rejected due to a conflict
        Conflict,
        /// if the command was invalid due to its parameters
        InvalidCommand,
        ///If the command was invalid due to the object state
        InvalidState,
        // Not handled
        NotHandled,
        //Entity was not found
        NotFound
    }
}

namespace EventDriven.CQRS.Abstractions.Entities
{
    /// <summary>
    ///     The state of an entity.
    /// </summary>
    public enum EntityState
    {
        /// <summary>
        ///     The entity is in an active state.
        /// </summary>
        Active,

        /// <summary>
        ///     The entity is in a frozen state.
        /// </summary>
        Frozen,

        /// <summary>
        ///     The entity is an a deleted state.
        /// </summary>
        Deleted
    }
}
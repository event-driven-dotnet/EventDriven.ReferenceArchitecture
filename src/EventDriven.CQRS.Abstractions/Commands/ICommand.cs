using System;

namespace EventDriven.CQRS.Abstractions.Commands
{
    /// <summary>
    /// Represents a command
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Represents the ID of the entity the command is in reference to.
        /// </summary>
        Guid EntityId { get; }

        /// <summary>
        /// If provided, refers to the version of the entity to update. 
        /// </summary>
        /// <remarks>If the eTag/version number does not match the current entity, then a conflict
        /// will be returned as a response to the command.</remarks>
        string EntityEtag { get; set; }

        /// <summary>
        /// If explicitly set to true, the entity must exist.  If the EntityETag is given, this will be 
        /// treated as if it were true.
        /// <ul>
        /// <li>ALWAYS Set to true or set the EntityEtag to a valid value if operating against an entity that is known to exist. </li>
        /// <li>ALWAYS Set to false if it is known the entity does not yet exist. </li>
        /// <li>Otherwise, If the value is null, and the EntityEtag is null, no assertion for the entity existing or not will be made. </li>
        /// </ul> 
        /// </summary>
        /// <value></value>
        bool? EntityExists { get; }
    }
}

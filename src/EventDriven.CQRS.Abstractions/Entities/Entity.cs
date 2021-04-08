using System;

namespace EventDriven.CQRS.Abstractions.Entities
{
    public abstract class Entity : IEntity
    {
        public Guid Id { get; set; }
        public long SequenceNumber { get; set; }
        public string ETag { get; set; }
        public EntityState State { get; set; } = EntityState.Active;

        protected void MarkDeleted()
        {
            State = EntityState.Deleted;
        }

        protected void Freeze()
        {
            State = EntityState.Frozen;
        }
    }
}
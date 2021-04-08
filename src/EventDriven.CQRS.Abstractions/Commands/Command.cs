using System;

namespace EventDriven.CQRS.Abstractions.Commands
{
    public record Command : ICommand
    {
        public Guid EntityId => default(Guid);
        public string EntityEtag { get; set; }
        public bool? EntityExists => false;

        public abstract record Create(
            Guid EntityId = default) : ICommand
        {
            public string EntityEtag { get; set; }
            public bool? EntityExists => false;
        }

        public abstract record Update(
            Guid EntityId,
            string EntityEtag = null) : ICommand
        {
            public string EntityEtag { get; set; } = EntityEtag;
            public bool? EntityExists => true;
        }

        public abstract record Remove(
            Guid EntityId,
            string EntityEtag = null) : ICommand
        {
            public string EntityEtag { get; set; } = EntityEtag;
            public bool? EntityExists => true;
        }
    }
}

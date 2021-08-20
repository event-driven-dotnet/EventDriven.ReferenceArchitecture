using System;
using EventDriven.CQRS.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.Commands
{
    public record RemoveCustomer(Guid EntityId) : Command.Remove(EntityId);
}
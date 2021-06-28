using System.Diagnostics.CodeAnalysis;
using EventDriven.CQRS.Abstractions.Events;

namespace CustomerService.Domain.CustomerAggregate.Events
{
    [ExcludeFromCodeCoverage]
    public record CustomerCreated(Customer Customer) : DomainEvent(Customer.Id);
}
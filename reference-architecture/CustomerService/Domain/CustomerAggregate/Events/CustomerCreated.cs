using EventDriven.CQRS.Abstractions.Events;

namespace CustomerService.Domain.CustomerAggregate.Events
{
    public record CustomerCreated(Customer Customer) : DomainEvent(Customer.Id);
}

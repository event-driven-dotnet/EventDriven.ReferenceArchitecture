namespace CustomerService.Domain.CustomerAggregate.Events {

    using System.Diagnostics.CodeAnalysis;
    using EventDriven.CQRS.Abstractions.Events;

    [ExcludeFromCodeCoverage]
    public record CustomerCreated(Customer Customer) : DomainEvent(Customer.Id);

}
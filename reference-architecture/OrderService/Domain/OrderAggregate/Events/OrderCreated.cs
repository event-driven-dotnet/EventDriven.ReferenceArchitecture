namespace OrderService.Domain.OrderAggregate.Events {

    using System.Diagnostics.CodeAnalysis;
    using EventDriven.CQRS.Abstractions.Events;

    [ExcludeFromCodeCoverage]
    public record OrderCreated(Order Order) : DomainEvent(Order.Id);

}
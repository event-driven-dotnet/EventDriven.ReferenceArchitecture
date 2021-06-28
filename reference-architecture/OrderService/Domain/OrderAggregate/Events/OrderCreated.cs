using System.Diagnostics.CodeAnalysis;
using EventDriven.CQRS.Abstractions.Events;

namespace OrderService.Domain.OrderAggregate.Events
{
    [ExcludeFromCodeCoverage]
    public record OrderCreated(Order Order) : DomainEvent(Order.Id);
}
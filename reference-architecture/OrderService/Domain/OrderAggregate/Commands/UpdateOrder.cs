using EventDriven.CQRS.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands
{
    public record UpdateOrder(Order Order) : Command.Update(Order.Id, Order.ETag);
}
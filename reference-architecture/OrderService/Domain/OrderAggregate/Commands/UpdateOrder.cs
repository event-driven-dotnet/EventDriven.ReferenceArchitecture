using EventDriven.DDD.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands
{
    public record UpdateOrder(Order Entity) : Command<Order>(Entity);
}
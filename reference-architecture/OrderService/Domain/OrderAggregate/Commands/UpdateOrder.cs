using EventDriven.DDD.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands
{
    public record UpdateOrder(Order Order) : Command(Order.Id);
}
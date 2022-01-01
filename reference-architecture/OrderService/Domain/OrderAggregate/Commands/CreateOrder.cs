using EventDriven.DDD.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands
{
    public record CreateOrder(Order Order) : Command(Order.Id);
}
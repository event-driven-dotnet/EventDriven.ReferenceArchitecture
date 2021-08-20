using EventDriven.CQRS.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands
{
    public record CreateOrder(Order Order) : Command.Create(Order.Id);
}
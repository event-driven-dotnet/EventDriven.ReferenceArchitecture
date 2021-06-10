namespace OrderService.Domain.OrderAggregate.Commands {

    using System;
    using EventDriven.CQRS.Abstractions.Commands;

    public record RemoveOrder(Guid OrderId) : ICommand<CommandResult<Order>>;

}
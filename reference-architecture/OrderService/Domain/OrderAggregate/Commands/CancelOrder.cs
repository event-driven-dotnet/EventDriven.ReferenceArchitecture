namespace OrderService.Domain.OrderAggregate.Commands {

    using System;
    using EventDriven.CQRS.Abstractions.Commands;

    public record CancelOrder(Guid EntityId, string ETag) : ICommand<CommandResult<Order>>;

}
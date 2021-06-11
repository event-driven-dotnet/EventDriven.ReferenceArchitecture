namespace OrderService.Domain.OrderAggregate.Commands {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using EventDriven.CQRS.Abstractions.Commands;

    [ExcludeFromCodeCoverage]
    public record RemoveOrder(Guid OrderId) : ICommand<CommandResult>;

}
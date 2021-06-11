namespace OrderService.Domain.OrderAggregate.Commands {

    using System.Diagnostics.CodeAnalysis;
    using EventDriven.CQRS.Abstractions.Commands;

    [ExcludeFromCodeCoverage]
    public record UpdateOrder(Order Order) : ICommand<CommandResult<Order>>;

}
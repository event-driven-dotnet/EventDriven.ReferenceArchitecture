namespace OrderService.Domain.OrderAggregate.Commands {

    using System.Diagnostics.CodeAnalysis;
    using EventDriven.CQRS.Abstractions.Commands;

    [ExcludeFromCodeCoverage]
    public record CreateOrder(Order Order) : ICommand<CommandResult<Order>>;

}
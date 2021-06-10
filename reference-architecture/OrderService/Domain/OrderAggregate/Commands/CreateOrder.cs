namespace OrderService.Domain.OrderAggregate.Commands {

    using EventDriven.CQRS.Abstractions.Commands;

    public record CreateOrder(Order Order) : ICommand<CommandResult<Order>>;

}
using System.Diagnostics.CodeAnalysis;
using EventDriven.CQRS.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands
{
    [ExcludeFromCodeCoverage]
    public record CreateOrder(Order Order) : ICommand<CommandResult<Order>>;
}
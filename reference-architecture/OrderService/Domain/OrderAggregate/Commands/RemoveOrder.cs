using System;
using System.Diagnostics.CodeAnalysis;
using EventDriven.CQRS.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands
{
    [ExcludeFromCodeCoverage]
    public record RemoveOrder(Guid OrderId) : ICommand<CommandResult>;
}
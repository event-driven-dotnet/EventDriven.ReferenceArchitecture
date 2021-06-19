using System;
using System.Diagnostics.CodeAnalysis;
using EventDriven.CQRS.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.Commands
{

    [ExcludeFromCodeCoverage]
    public record RemoveCustomer(Guid CustomerId) : ICommand<CommandResult>;

}
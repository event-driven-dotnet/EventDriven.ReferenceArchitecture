namespace CustomerService.Domain.CustomerAggregate.Commands
{

    using System;
    using System.Diagnostics.CodeAnalysis;
    using EventDriven.CQRS.Abstractions.Commands;

    [ExcludeFromCodeCoverage]
    public record RemoveCustomer(Guid CustomerId) : ICommand<CommandResult>;

}
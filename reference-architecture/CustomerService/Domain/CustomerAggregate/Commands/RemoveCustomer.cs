namespace CustomerService.Domain.CustomerAggregate.Commands
{

    using System;
    using EventDriven.CQRS.Abstractions.Commands;

    public record RemoveCustomer(Guid CustomerId) : ICommand<CommandResult<Customer>>;

}
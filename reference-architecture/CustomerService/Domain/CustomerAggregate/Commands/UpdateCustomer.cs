namespace CustomerService.Domain.CustomerAggregate.Commands
{

    using System;
    using EventDriven.CQRS.Abstractions.Commands;

    public record UpdateCustomer(Customer Customer, Guid CustomerId, string ETag) : ICommand<CommandResult<Customer>> {
        public UpdateCustomer(Customer customer) : this(customer, customer.Id, customer.ETag) { }
    }

}
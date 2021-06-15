namespace CustomerService.Domain.CustomerAggregate {

    using Commands;
    using EventDriven.CQRS.Abstractions.Commands;
    using EventDriven.CQRS.Abstractions.Events;
    using Events;

    public interface ICustomerAggregate : ICommandProcessor<CreateCustomer, CommandResult<Customer>>,
                                          IEventApplier<CustomerCreated> { }

}
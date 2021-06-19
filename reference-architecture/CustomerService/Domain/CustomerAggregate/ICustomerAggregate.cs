using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Domain.CustomerAggregate.Events;
using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.CQRS.Abstractions.Events;

namespace CustomerService.Domain.CustomerAggregate
{

    public interface ICustomerAggregate : ICommandProcessor<CreateCustomer, CommandResult<Customer>>,
                                          IEventApplier<CustomerCreated> { }

}
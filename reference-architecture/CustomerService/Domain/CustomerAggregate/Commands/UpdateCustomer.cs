using EventDriven.CQRS.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.Commands
{
    public record UpdateCustomer(Customer Customer) : Command.Update(Customer.Id, Customer.ETag);
}
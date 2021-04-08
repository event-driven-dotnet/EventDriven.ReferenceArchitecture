using EventDriven.CQRS.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.Commands
{
    public record CreateCustomer(Customer Customer) : Command.Create(Customer.Id);
}
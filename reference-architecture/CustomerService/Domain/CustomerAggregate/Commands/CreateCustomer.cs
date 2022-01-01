using EventDriven.DDD.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.Commands
{
    public record CreateCustomer(Customer Customer) : Command(Customer.Id);
}
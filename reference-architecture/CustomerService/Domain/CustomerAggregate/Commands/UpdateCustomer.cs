using EventDriven.DDD.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.Commands
{
    public record UpdateCustomer(Customer Customer) : Command(Customer.Id);
}
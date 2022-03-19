using EventDriven.CQRS.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.Commands;

public record UpdateCustomer(Customer? Entity) : Command<Customer>(Entity);
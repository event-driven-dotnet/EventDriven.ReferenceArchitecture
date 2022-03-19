using EventDriven.CQRS.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.Commands;

public record CreateCustomer(Customer? Entity) : Command<Customer>(Entity);
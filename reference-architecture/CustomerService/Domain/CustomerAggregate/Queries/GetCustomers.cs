using EventDriven.CQRS.Abstractions.Queries;

namespace CustomerService.Domain.CustomerAggregate.Queries;

public record GetCustomers : Query<IEnumerable<Customer>>;
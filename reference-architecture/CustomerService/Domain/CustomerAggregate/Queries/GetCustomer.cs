using EventDriven.CQRS.Abstractions.Queries;

namespace CustomerService.Domain.CustomerAggregate.Queries;

public record GetCustomer(Guid Id) : Query<Customer?>;
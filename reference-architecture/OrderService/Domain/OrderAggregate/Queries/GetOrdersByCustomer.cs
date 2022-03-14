using EventDriven.CQRS.Abstractions.Queries;

namespace OrderService.Domain.OrderAggregate.Queries;

public record GetOrdersByCustomer(Guid CustomerId) : Query<IEnumerable<Order>>;
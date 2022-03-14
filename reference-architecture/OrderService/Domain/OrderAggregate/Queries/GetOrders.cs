using EventDriven.CQRS.Abstractions.Queries;

namespace OrderService.Domain.OrderAggregate.Queries;

public record GetOrders : Query<IEnumerable<Order>>;
using EventDriven.CQRS.Abstractions.Queries;

namespace OrderService.Domain.OrderAggregate.Queries;

public record GetOrder(Guid Id) : Query<Order?>;
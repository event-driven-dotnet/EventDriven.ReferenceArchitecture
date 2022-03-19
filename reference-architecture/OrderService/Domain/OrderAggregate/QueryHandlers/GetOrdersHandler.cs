using EventDriven.CQRS.Abstractions.Queries;
using OrderService.Domain.OrderAggregate.Queries;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.QueryHandlers;

public class GetOrdersHandler : IQueryHandler<GetOrders, IEnumerable<Order>>
{
    private readonly IOrderRepository _repository;

    public GetOrdersHandler(
        IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Order>> Handle(GetOrders query, CancellationToken cancellationToken)
    {
        // Retrieve entities
        var result = await _repository.GetAsync();
        return result;
    }
}
using EventDriven.CQRS.Abstractions.Queries;
using OrderService.Domain.OrderAggregate.Queries;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.QueryHandlers;

public class GetOrderHandler : IQueryHandler<GetOrder, Order?>
{
    private readonly IOrderRepository _repository;

    public GetOrderHandler(
        IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<Order?> Handle(GetOrder query, CancellationToken cancellationToken)
    {
        var result = await _repository.GetAsync(query.Id);
        return result;
    }
}
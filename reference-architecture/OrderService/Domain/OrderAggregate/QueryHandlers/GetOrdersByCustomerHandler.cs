using EventDriven.CQRS.Abstractions.Queries;
using OrderService.Domain.OrderAggregate.Queries;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.QueryHandlers;

public class GetOrdersByCustomerHandler : IQueryHandler<GetOrdersByCustomer, IEnumerable<Order>>
{
    private readonly IOrderRepository _repository;

    public GetOrdersByCustomerHandler(
        IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Order>> Handle(GetOrdersByCustomer query, CancellationToken cancellationToken)
    {
        // Retrieve entities
        var result = await _repository.GetByCustomerAsync(query.CustomerId);
        return result;
    }
}
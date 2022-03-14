using EventDriven.CQRS.Abstractions.Queries;
using OrderService.Domain.OrderAggregate.Queries;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.QueryHandlers;

public class GetOrdersByCustomerHandler : IQueryHandler<GetOrdersByCustomer, IEnumerable<Order>>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<GetOrdersByCustomerHandler> _logger;

    public GetOrdersByCustomerHandler(
        IOrderRepository repository,
        ILogger<GetOrdersByCustomerHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<Order>> Handle(GetOrdersByCustomer query, CancellationToken cancellationToken)
    {
        // Retrieve entities
        _logger.LogInformation("Handling command: {CommandName}", nameof(GetOrdersByCustomer));
        var result = await _repository.GetByCustomerAsync(query.CustomerId);
        return result;
    }
}
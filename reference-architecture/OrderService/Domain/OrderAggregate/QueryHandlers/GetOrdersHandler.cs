using EventDriven.CQRS.Abstractions.Queries;
using OrderService.Domain.OrderAggregate.Queries;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.QueryHandlers;

public class GetOrdersHandler : IQueryHandler<GetOrders, IEnumerable<Order>>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<GetOrdersHandler> _logger;

    public GetOrdersHandler(
        IOrderRepository repository,
        ILogger<GetOrdersHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<Order>> Handle(GetOrders query, CancellationToken cancellationToken)
    {
        // Retrieve entities
        _logger.LogInformation("Handling command: {CommandName}", nameof(GetOrders));
        var result = await _repository.GetAsync();
        return result;
    }
}
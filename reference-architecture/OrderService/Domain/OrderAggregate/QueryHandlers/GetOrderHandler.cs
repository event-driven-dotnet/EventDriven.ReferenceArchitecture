using EventDriven.CQRS.Abstractions.Queries;
using OrderService.Domain.OrderAggregate.Queries;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.QueryHandlers;

public class GetOrderHandler : IQueryHandler<GetOrder, Order?>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<GetOrderHandler> _logger;

    public GetOrderHandler(
        IOrderRepository repository,
        ILogger<GetOrderHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Order?> Handle(GetOrder query, CancellationToken cancellationToken)
    {
        // Retrieve entities
        _logger.LogInformation("Handling command: {CommandName}", nameof(GetOrder));
        var result = await _repository.GetAsync(query.Id);
        return result;
    }
}
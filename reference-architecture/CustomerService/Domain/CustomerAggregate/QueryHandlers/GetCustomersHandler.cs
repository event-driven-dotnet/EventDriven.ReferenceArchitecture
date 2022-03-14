using CustomerService.Domain.CustomerAggregate.Queries;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Queries;

namespace CustomerService.Domain.CustomerAggregate.QueryHandlers;

public class GetCustomersHandler : IQueryHandler<GetCustomers, IEnumerable<Customer>>
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<GetCustomersHandler> _logger;

    public GetCustomersHandler(
        ICustomerRepository repository,
        ILogger<GetCustomersHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<Customer>> Handle(GetCustomers query, CancellationToken cancellationToken)
    {
        // Retrieve entities
        _logger.LogInformation("Handling command: {CommandName}", nameof(GetCustomers));
        var result = await _repository.GetAsync();
        return result;
    }
}
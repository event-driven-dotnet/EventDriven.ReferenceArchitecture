using CustomerService.Domain.CustomerAggregate.Queries;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Queries;

namespace CustomerService.Domain.CustomerAggregate.QueryHandlers;

public class GetCustomerHandler : IQueryHandler<GetCustomer, Customer?>
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<GetCustomerHandler> _logger;

    public GetCustomerHandler(
        ICustomerRepository repository,
        ILogger<GetCustomerHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Customer?> Handle(GetCustomer query, CancellationToken cancellationToken)
    {
        // Retrieve entity
        _logger.LogInformation("Handling command: {CommandName}", nameof(GetCustomers));
        var result = await _repository.GetAsync(query.Id);
        return result;
    }
}
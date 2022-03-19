using CustomerService.Domain.CustomerAggregate.Queries;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Queries;

namespace CustomerService.Domain.CustomerAggregate.QueryHandlers;

public class GetCustomerHandler : IQueryHandler<GetCustomer, Customer?>
{
    private readonly ICustomerRepository _repository;

    public GetCustomerHandler(
        ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<Customer?> Handle(GetCustomer query, CancellationToken cancellationToken)
    {
        var result = await _repository.GetAsync(query.Id);
        return result;
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerService.Domain.CustomerAggregate;

namespace CustomerService.Repositories
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> Get();
        Task<Customer> Get(Guid id);
        Task<Customer> Add(Customer entity);
        Task<Customer> Update(Customer entity);
        Task<int> Remove(Guid id);
    }
}
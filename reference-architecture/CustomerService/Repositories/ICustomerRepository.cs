using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerService.Domain.CustomerAggregate;

namespace CustomerService.Repositories
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAsync();
        Task<Customer> GetAsync(Guid id);
        Task<Customer> AddAsync(Customer entity);
        Task<Customer> UpdateAsync(Customer entity);
        Task<int> RemoveAsync(Guid id);
    }
}
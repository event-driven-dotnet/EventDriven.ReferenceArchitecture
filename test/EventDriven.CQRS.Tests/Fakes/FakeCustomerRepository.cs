using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Repositories;

namespace EventDriven.CQRS.Tests.Fakes
{
    public class FakeCustomerRepository : ICustomerRepository
    {
        private readonly Dictionary<Guid, Customer> _entities = new();

        public Task<IEnumerable<Customer>> GetAsync() =>
            Task.FromResult(_entities.Values.AsEnumerable());

        public Task<Customer> GetAsync(Guid id) =>
            _entities.TryGetValue(id, out var entity)
                ? Task.FromResult(entity)
                : Task.FromResult<Customer>(null);

        public Task<Customer> AddAsync(Customer entity)
        {
            if (_entities.ContainsKey(entity.Id))
                return Task.FromResult<Customer>(null);
            entity.ETag = Guid.NewGuid().ToString();
            _entities.Add(entity.Id, entity);
            return Task.FromResult(entity);
        }

        public Task<Customer> UpdateAsync(Customer entity)
        {
            if (!_entities.TryGetValue(entity.Id, out var existing))
                return Task.FromResult<Customer>(null);
            if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0 )
                throw new ConcurrencyException();
            existing.ETag = Guid.NewGuid().ToString();
            existing.FirstName = entity.FirstName;
            existing.LastName = entity.LastName;
            existing.ShippingAddress = entity.ShippingAddress;
            return Task.FromResult(existing);
        }

        public Task<int> RemoveAsync(Guid id)
        {
            if (!_entities.ContainsKey(id))
                return Task.FromResult(0);
            var result = _entities.Remove(id) ? 1 : 0;
            return Task.FromResult(result);
        }
    }
}

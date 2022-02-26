using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.Domain.OrderAggregate;
using OrderService.Repositories;
using ConcurrencyException = CustomerService.Repositories.ConcurrencyException;

namespace EventDriven.CQRS.Tests.Fakes
{
    public class FakeOrderRepository : IOrderRepository
    {
        private readonly Dictionary<Guid, Order> _entities = new();

        public Task<IEnumerable<Order>> GetAsync() =>
            Task.FromResult(_entities.Values.AsEnumerable());

        public Task<IEnumerable<Order>> GetByCustomerAsync(Guid customerId) =>
            Task.FromResult(_entities.Values.AsEnumerable().Where(o => o.CustomerId == customerId));

        public Task<Order> GetAsync(Guid id)
        {
            if (_entities.TryGetValue(id, out var entity))
                return Task.FromResult(entity);
            else return Task.FromResult<Order>(null);
        }

        public Task<Order> AddAsync(Order entity)
        {
            if (_entities.ContainsKey(entity.Id))
                return Task.FromResult<Order>(null);
            entity.ETag = Guid.NewGuid().ToString();
            _entities.Add(entity.Id, entity);
            return Task.FromResult(entity);
        }

        public Task<Order> UpdateAsync(Order entity)
        {
            if (!_entities.TryGetValue(entity.Id, out var existing))
                return Task.FromResult<Order>(null);
            if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0 )
                throw new ConcurrencyException();
            existing.ETag = Guid.NewGuid().ToString();
            existing.CustomerId = entity.CustomerId;
            existing.OrderDate = entity.OrderDate;
            existing.OrderItems = entity.OrderItems;
            existing.ShippingAddress = entity.ShippingAddress;
            existing.OrderState = entity.OrderState;
            return Task.FromResult(existing);
        }

        public Task<Order> UpdateAddressAsync(Guid orderId, Address address)
        {
            if (!_entities.TryGetValue(orderId, out var existing))
                return Task.FromResult<Order>(null);
            existing.ShippingAddress = address;
            return Task.FromResult(existing);
        }

        public Task<int> RemoveAsync(Guid id)
        {
            if (!_entities.ContainsKey(id))
                return Task.FromResult(0);
            var result = _entities.Remove(id) ? 1 : 0;
            return Task.FromResult(result);
        }

        public Task<Order> UpdateOrderStateAsync(Order entity, OrderState orderState)
        {
            if (!_entities.TryGetValue(entity.Id, out var existing))
                return Task.FromResult<Order>(null);
            if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0 )
                throw new ConcurrencyException();
            existing.ETag = Guid.NewGuid().ToString();
            existing.OrderState = orderState;
            return Task.FromResult(existing);
        }
    }
}

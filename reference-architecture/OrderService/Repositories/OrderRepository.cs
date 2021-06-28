using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrderService.Domain.OrderAggregate;
using URF.Core.Abstractions;

namespace OrderService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDocumentRepository<Order> _documentRepository;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(IDocumentRepository<Order> documentRepository,
                               ILogger<OrderRepository> logger)
        {
            _documentRepository = documentRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Order>> GetOrders() =>
            await _documentRepository.FindManyAsync();

        public async Task<IEnumerable<Order>> GetCustomerOrders(Guid customerId) =>
            await _documentRepository.FindManyAsync(e => e.CustomerId == customerId);

        public async Task<Order> GetOrder(Guid id) =>
            await _documentRepository.FindOneAsync(e => e.Id == id);

        public async Task<Order> AddOrder(Order entity)
        {
            var existing = await _documentRepository.FindOneAsync(e => e.Id == entity.Id);
            if (existing != null) return null;

            entity.SequenceNumber = 1;
            entity.ETag = Guid.NewGuid()
                              .ToString();
            return await _documentRepository.InsertOneAsync(entity);
        }

        public async Task<Order> UpdateOrder(Order entity)
        {
            var existing = await GetOrder(entity.Id);
            if (existing == null) return null;

            if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0)
                throw new ConcurrencyException();

            entity.SequenceNumber = existing.SequenceNumber + 1;
            entity.ETag = Guid.NewGuid()
                              .ToString();
            return await _documentRepository.FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
        }

        public async Task<Order> UpdateOrderAddress(Guid orderId, Address address)
        {
            var existing = await GetOrder(orderId);
            if (existing == null) return null;

            existing.ShippingAddress = address;
            return await _documentRepository.FindOneAndReplaceAsync(e => e.Id == orderId, existing);
        }

        public async Task<int> RemoveOrder(Guid id) =>
            await _documentRepository.DeleteOneAsync(e => e.Id == id);

        public async Task<Order> UpdateOrderState(Order entity, OrderState orderState)
        {
            var existing = await GetOrder(entity.Id);
            if (existing == null) return null;

            if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0)
                throw new ConcurrencyException();

            entity.SequenceNumber++;
            entity.ETag = Guid.NewGuid()
                              .ToString();
            entity.OrderState = orderState;
            return await _documentRepository.FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
        }
    }
}
using System.Diagnostics.CodeAnalysis;
using EventDriven.DDD.Abstractions.Repositories;
using MongoDB.Driver;
using OrderService.Domain.OrderAggregate;
using URF.Core.Mongo;

namespace OrderService.Repositories;

[ExcludeFromCodeCoverage]
public class OrderRepository : DocumentRepository<Order>, IOrderRepository
{
    public OrderRepository(IMongoCollection<Order> collection) : base(collection)
    {
    }
    public async Task<IEnumerable<Order>> GetAsync() => 
        await FindManyAsync();

    public async Task<IEnumerable<Order>> GetByCustomerAsync(Guid customerId) =>
        await FindManyAsync(e => e.CustomerId == customerId);

    public async Task<Order?> GetAsync(Guid id) =>
        await FindOneAsync(e => e.Id == id);

    public async Task<Order?> AddAsync(Order entity)
    {
        var existing = await FindOneAsync(e => e.Id == entity.Id);
        if (existing != null) return null;
        if (string.IsNullOrWhiteSpace(entity.ETag))
            entity.ETag = Guid.NewGuid().ToString();
        return await InsertOneAsync(entity);
    }

    public async Task<Order?> UpdateAsync(Order entity)
    {
        var existing = await GetAsync(entity.Id);
        if (existing == null) return null;
        if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0 )
            throw new ConcurrencyException();
        entity.ETag = Guid.NewGuid().ToString();
        return await FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
    }

    public async Task<Order?> UpdateAddressAsync(Guid orderId, Address address)
    {
        var existing = await GetAsync(orderId);
        if (existing == null) return null;
        existing.ShippingAddress = address;
        return await FindOneAndReplaceAsync(e => e.Id == orderId, existing);
    }

    public async Task<int> RemoveAsync(Guid id) =>
        await DeleteOneAsync(e => e.Id == id);

    public async Task<Order?> UpdateOrderStateAsync(Order entity, OrderState orderState)
    {
        var existing = await GetAsync(entity.Id);
        if (existing == null) return null;
        if (string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0 )
            throw new ConcurrencyException();
        entity.ETag = Guid.NewGuid().ToString();
        entity.OrderState = orderState;
        return await FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
    }
}
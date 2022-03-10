using OrderService.Domain.OrderAggregate;

namespace OrderService.Repositories;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAsync();
    Task<IEnumerable<Order>> GetByCustomerAsync(Guid customerId);
    Task<Order?> GetAsync(Guid id);
    Task<Order?> AddAsync(Order entity);
    Task<Order?> UpdateAsync(Order entity);
    Task<Order?> UpdateAddressAsync(Guid orderId, Address address);
    Task<int> RemoveAsync(Guid id);
    Task<Order?> UpdateOrderStateAsync(Order entity, OrderState orderState);
}
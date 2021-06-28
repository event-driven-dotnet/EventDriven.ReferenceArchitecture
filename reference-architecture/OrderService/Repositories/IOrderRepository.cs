using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderService.Domain.OrderAggregate;

namespace OrderService.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrders();
        Task<IEnumerable<Order>> GetCustomerOrders(Guid customerId);
        Task<Order> GetOrder(Guid id);
        Task<Order> AddOrder(Order entity);
        Task<Order> UpdateOrder(Order entity);
        Task<Order> UpdateOrderAddress(Guid orderId, Address address);
        Task<int> RemoveOrder(Guid id);
        Task<Order> UpdateOrderState(Order entity, OrderState orderState);
    }
}
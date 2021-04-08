using System;
using System.Collections.Generic;

namespace OrderService.DTO.Write
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public Address ShippingAddress { get; set; }
        public OrderState OrderState { get; set; }
        public string ETag { get; set; }
    }
}
using System;
using System.Diagnostics.CodeAnalysis;

namespace OrderService.DTO.Read
{
    [ExcludeFromCodeCoverage]
    public class OrderView
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderTotal { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public OrderState OrderState { get; set; }
        public string ETag { get; set; }
    }
}
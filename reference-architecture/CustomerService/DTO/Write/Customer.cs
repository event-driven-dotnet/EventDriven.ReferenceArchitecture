using System;

namespace CustomerService.DTO.Write
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address ShippingAddress { get; set; }
        public string ETag { get; set; }
    }
}
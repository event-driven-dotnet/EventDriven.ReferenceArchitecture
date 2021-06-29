using System;
using System.Diagnostics.CodeAnalysis;

namespace CustomerService.DTO.Read
{
    [ExcludeFromCodeCoverage]
    public class CustomerView
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string ETag { get; set; }
    }
}
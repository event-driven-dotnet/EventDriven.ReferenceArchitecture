using System;
using CustomerService.DTO.Read;
using CustomerService.DTO.Write;

namespace CustomerService.Tests.Fakes;

public static class Customers
{
    public static Customer Customer1 => new()
    {
        Id = Guid.Parse("22eea083-6f0d-48f2-8c82-65ac850e5aad"),
        FirstName = "Elon",
        LastName = "Musk",
        ShippingAddress = new Address
        {
            Street = "123 This Street",
            City = "Freemont",
            State = "CA",
            Country = "USA",
            PostalCode = "90045"
        }
    };

    public static Customer Customer2 => new()
    {
        Id = Guid.Parse("848f5790-3981-4862-bb7e-a8566aa07026"),
        FirstName = "Jeff",
        LastName = "Bezos",
        ShippingAddress = new Address
        {
            Street = "123 That Street",
            City = "Seattle",
            State = "WA",
            Country = "USA",
            PostalCode = "90045"
        }
    };

    public static Customer Customer3 => new()
    {
        Id = Guid.Parse("1c44eea7-400a-4f6f-ab99-5e8c853ea363"),
        FirstName = "Mark",
        LastName = "Zuckerberg",
        ShippingAddress = new Address
        {
            Street = "123 Other Street",
            City = "Palo Alto",
            State = "CA",
            Country = "USA",
            PostalCode = "98765"
        }
    };
}

public static class CustomerViews
{
    public static CustomerView Customer1 => new()
    {
        Id = Guid.Parse("22eea083-6f0d-48f2-8c82-65ac850e5aad"),
        FirstName = "Elon",
        LastName = "Musk",
        Street = "123 This Street",
        City = "Freemont",
        State = "CA",
        Country = "USA",
        PostalCode = "90045"
    };

    public static CustomerView Customer2 => new()
    {
        Id = Guid.Parse("848f5790-3981-4862-bb7e-a8566aa07026"),
        FirstName = "Jeff",
        LastName = "Bezos",
        Street = "123 That Street",
        City = "Seattle",
        State = "WA",
        Country = "USA",
        PostalCode = "90045"
    };

    public static CustomerView Customer3 => new()
    {
        Id = Guid.Parse("1c44eea7-400a-4f6f-ab99-5e8c853ea363"),
        FirstName = "Mark",
        LastName = "Zuckerberg",
        Street = "123 Other Street",
        City = "Palo Alto",
        State = "CA",
        Country = "USA",
        PostalCode = "98765"
    };
}
using System;
using System.Collections.Generic;
using OrderService.DTO.Read;
using OrderService.DTO.Write;
using OrderState = OrderService.DTO.Read.OrderState;

namespace OrderService.Tests.Fakes;

using ReadOrderState = OrderState;
using WriteOrderState = DTO.Write.OrderState;

public static class Orders
{
    public static Order Order1 => new()
    {
        Id = Guid.Parse("22eea083-6f0d-48f2-8c82-65ac850e5aad"),
        CustomerId = Guid.Parse("22eea083-6f0d-48f2-8c82-65ac850e5aad"),
        OrderDate = DateTime.Now,
        OrderItems = new List<OrderItem>
        {
            new(Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), "Espresso", 2.5M),
            new(Guid.Parse("2b3adade-9499-4673-8e0a-8e4b5e91ddb1"), "Cappuccino", 3.5M),
            new(Guid.Parse("2d14cb8b-7400-4843-b4be-6b6ec68ee654"), "Latte", 4.5M)
        },
        ShippingAddress = new Address
        {
            Street = "123 This Street",
            City = "Freemont",
            State = "CA",
            Country = "USA",
            PostalCode = "90045"
        },
        OrderState = WriteOrderState.Created
    };

    public static Order Order2 => new()
    {
        Id = Guid.Parse("39949530-cc63-4aaa-bc65-49cdfd888b17"),
        CustomerId = Guid.Parse("22eea083-6f0d-48f2-8c82-65ac850e5aad"),
        OrderDate = DateTime.Now,
        OrderItems = new List<OrderItem>
        {
            new(Guid.Parse("6875b6dd-2e00-460d-aa93-695852dcc1e8"), "Coke", 2.5M),
            new(Guid.Parse("cda037c0-688a-408c-9eb4-33c9b580d260"), "Squirt", 3.5M),
            new(Guid.Parse("b26462b9-0efa-49e1-8639-9a80ed47829c"), "Fresca", 4.5M)
        },
        ShippingAddress = new Address
        {
            Street = "123 That Street",
            City = "Dallas",
            State = "TX",
            Country = "USA",
            PostalCode = "70023"
        },
        OrderState = WriteOrderState.Created
    };
}

public static class OrderViews
{
    public static OrderView Order1 => new()
    {
        Id = Guid.Parse("22eea083-6f0d-48f2-8c82-65ac850e5aad"),
        CustomerId = Guid.Parse("22eea083-6f0d-48f2-8c82-65ac850e5aad"),
        OrderDate = DateTime.Now,
        OrderTotal = 212,
        Street = "123 This Street",
        City = "Freemont",
        State = "CA",
        Country = "USA",
        PostalCode = "90045",
        OrderState = ReadOrderState.Created
    };

    public static OrderView Order2 => new()
    {
        Id = Guid.Parse("39949530-cc63-4aaa-bc65-49cdfd888b17"),
        CustomerId = Guid.Parse("22eea083-6f0d-48f2-8c82-65ac850e5aad"),
        OrderDate = DateTime.Now,
        OrderTotal = 212,
        Street = "123 That Street",
        City = "Dallas",
        State = "TX",
        Country = "USA",
        PostalCode = "70023",
        OrderState = ReadOrderState.Created
    };
}
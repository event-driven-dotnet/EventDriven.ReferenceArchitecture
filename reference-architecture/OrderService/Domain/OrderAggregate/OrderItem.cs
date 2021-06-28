using System;
using System.Diagnostics.CodeAnalysis;

namespace OrderService.Domain.OrderAggregate
{
    [ExcludeFromCodeCoverage]
    public record OrderItem(Guid ProductId, string ProductName, decimal ProductPrice);
}
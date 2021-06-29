using System;
using System.Diagnostics.CodeAnalysis;

namespace OrderService.DTO.Write
{
    [ExcludeFromCodeCoverage]
    public record OrderItem(Guid ProductId, string ProductName, decimal ProductPrice);
}
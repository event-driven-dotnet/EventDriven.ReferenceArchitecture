namespace OrderService.DTO.Write {

    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public record OrderItem(Guid ProductId, string ProductName, decimal ProductPrice);

}
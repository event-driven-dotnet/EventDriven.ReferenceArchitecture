namespace OrderService.DTO.Write;

public record OrderItem(Guid ProductId, string ProductName, decimal ProductPrice);
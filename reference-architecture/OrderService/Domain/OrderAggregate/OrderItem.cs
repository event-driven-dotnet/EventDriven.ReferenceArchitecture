namespace OrderService.Domain.OrderAggregate;

public record OrderItem(Guid ProductId, string ProductName, decimal ProductPrice);
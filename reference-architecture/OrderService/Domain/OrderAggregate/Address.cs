using System.Diagnostics.CodeAnalysis;

namespace OrderService.Domain.OrderAggregate
{
    [ExcludeFromCodeCoverage]
    public record Address(string Street, string City, string State, string Country, string PostalCode);
}
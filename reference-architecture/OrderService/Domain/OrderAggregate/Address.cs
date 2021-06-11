namespace OrderService.Domain.OrderAggregate {

    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public record Address(string Street, string City, string State, string Country, string PostalCode);

}
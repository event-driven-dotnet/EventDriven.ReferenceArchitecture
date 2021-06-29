using System.Diagnostics.CodeAnalysis;

namespace CustomerService.Domain.CustomerAggregate
{
    [ExcludeFromCodeCoverage]
    public record Address(string Street, string City, string State, string Country, string PostalCode);
}
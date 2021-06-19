using System.Diagnostics.CodeAnalysis;
using EventDriven.CQRS.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.Commands
{

    [ExcludeFromCodeCoverage]
    public record CreateCustomer(Customer Customer) : ICommand<CommandResult<Customer>> { }

}
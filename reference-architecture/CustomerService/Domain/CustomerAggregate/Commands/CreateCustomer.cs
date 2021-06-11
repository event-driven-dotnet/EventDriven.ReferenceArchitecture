namespace CustomerService.Domain.CustomerAggregate.Commands {

    using System.Diagnostics.CodeAnalysis;
    using EventDriven.CQRS.Abstractions.Commands;

    [ExcludeFromCodeCoverage]
    public record CreateCustomer(Customer Customer): ICommand<CommandResult<Customer>> { }

}
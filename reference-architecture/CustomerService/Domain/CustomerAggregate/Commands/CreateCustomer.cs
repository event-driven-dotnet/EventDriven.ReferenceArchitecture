namespace CustomerService.Domain.CustomerAggregate.Commands {

    using EventDriven.CQRS.Abstractions.Commands;

    public record CreateCustomer(Customer Customer): ICommand<CommandResult<Customer>> { }

}
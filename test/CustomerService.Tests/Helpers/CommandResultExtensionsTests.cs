using CustomerService.Domain.CustomerAggregate;
using CustomerService.Helpers;
using CustomerService.Tests.Fakes;
using EventDriven.CQRS.Abstractions.Commands;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace CustomerService.Tests.Helpers;

public class CommandResultExtensionsTests
{
    [Fact]
    public void WhenAccepted_AndEntityIsNotPresent_ThenOkResult()
    {
        var cmdResult = new CommandResult(CommandOutcome.Accepted);

        var actionResult = cmdResult.ToActionResult();

        Assert.NotNull(actionResult);
        Assert.IsType<OkResult>(actionResult);
    }

    [Fact]
    public void WhenAccepted_AndEntityIsPresent_ThenOkObjectResult()
    {
        var mapper = MappingHelper.GetMapper();
        var cmdResult = new CommandResult(CommandOutcome.Accepted);

        var actionResult = cmdResult.ToActionResult(mapper.Map<Customer>(Customers.Customer1));
        var okResult = Assert.IsType<OkObjectResult>(actionResult);

        Assert.NotNull(actionResult);
        Assert.NotNull(okResult);
        Assert.NotNull(okResult.Value as Customer);
    }

    [Fact]
    public void WhenConflict_ThenConflictResult()
    {
        var cmdResult = new CommandResult(CommandOutcome.Conflict);

        var actionResult = cmdResult.ToActionResult();

        Assert.NotNull(actionResult);
        Assert.IsType<ConflictResult>(actionResult);
    }

    [Fact]
    public void WhenNotFound_ThenNotFoundResult()
    {
        var cmdResult = new CommandResult(CommandOutcome.NotFound);

        var actionResult = cmdResult.ToActionResult();

        Assert.NotNull(actionResult);
        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Fact]
    public void WhenNotExplicitlyCovered_Then500StatusCodeResult()
    {
        var cmdResult = new CommandResult(CommandOutcome.NotHandled);

        var actionResult = cmdResult.ToActionResult();

        Assert.NotNull(actionResult);
        var statusCodeResult = Assert.IsType<StatusCodeResult>(actionResult);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}
using System;
using System.Threading.Tasks;
using AutoMapper;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Domain.CustomerAggregate.Queries;
using CustomerService.Domain.CustomerAggregate.QueryHandlers;
using CustomerService.Repositories;
using CustomerService.Tests.Fakes;
using CustomerService.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CustomerService.Tests.Domain.CustomerAggregate.QueryHandlers;

public class GetCustomerHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<GetCustomerHandler>> _loggerMock;
    private readonly Mock<ICustomerRepository> _repositoryMock;

    public GetCustomerHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetCustomerHandler>>();
        _repositoryMock = new Mock<ICustomerRepository>();
        _mapper = MappingHelper.GetMapper();
    }

    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var handler = new GetCustomerHandler(_repositoryMock.Object, _loggerMock.Object);

        Assert.NotNull(handler);
        Assert.IsType<GetCustomerHandler>(handler);
    }

    [Fact]
    public async Task WhenRetrievingEntity_ThenEntityShouldBeReturned()
    {
        var expected = _mapper.Map<Customer>(Customers.Customer1);
        _repositoryMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expected);

        var handler = new GetCustomerHandler(_repositoryMock.Object, _loggerMock.Object);

        var actual = await handler.Handle(new GetCustomer(Customers.Customer1.Id), default);

        Assert.Equal(expected, actual);
    }
}
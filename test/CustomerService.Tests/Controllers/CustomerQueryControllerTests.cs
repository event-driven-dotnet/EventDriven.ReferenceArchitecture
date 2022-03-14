using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CustomerService.Controllers;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Domain.CustomerAggregate.Queries;
using CustomerService.DTO.Read;
using CustomerService.Tests.Fakes;
using CustomerService.Tests.Helpers;
using EventDriven.CQRS.Abstractions.Queries;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CustomerService.Tests.Controllers;

public class CustomerQueryControllerTests
{
    private readonly Mock<IQueryBroker> _queryBrokerMoq;
    private readonly IMapper _mapper;

    public CustomerQueryControllerTests()
    {
        _queryBrokerMoq = new Mock<IQueryBroker>();
        _mapper = MappingHelper.GetMapper();
    }

    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var controller = new CustomerQueryController(_queryBrokerMoq.Object, _mapper);

        Assert.IsAssignableFrom<ControllerBase>(controller);
        Assert.IsType<CustomerQueryController>(controller);
    }

    [Fact]
    public async Task WhenRetrievingAllCustomers_ThenAllCustomersShouldBeReturned()
    {
        _queryBrokerMoq.Setup(x => x.SendAsync(It.IsAny<GetCustomers>()))
            .ReturnsAsync(new List<Customer>
            {
                _mapper.Map<Customer>(Customers.Customer1),
                _mapper.Map<Customer>(Customers.Customer2),
                _mapper.Map<Customer>(Customers.Customer3)
            });

        var controller = new CustomerQueryController(_queryBrokerMoq.Object, _mapper);

        var actionResult = await controller.GetCustomers();
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var value = (IEnumerable<CustomerView>)okResult.Value!;

        Assert.Collection(value,
            c => Assert.Equal(CustomerViews.Customer1.Id, c.Id),
            c => Assert.Equal(CustomerViews.Customer2.Id, c.Id),
            c => Assert.Equal(CustomerViews.Customer3.Id, c.Id));
    }

    [Fact]
    public async Task GivenWeAreRetrievingACustomerById_WhenSuccessful_ThenCorrectCustomerShouldBeReturned()
    {
        _queryBrokerMoq.Setup(x => x.SendAsync(It.IsAny<GetCustomer>()))
            .ReturnsAsync(_mapper.Map<Customer>(Customers.Customer1));

        var controller = new CustomerQueryController(_queryBrokerMoq.Object, _mapper);

        var actionResult = await controller.GetCustomer(Customers.Customer1.Id);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var value = (CustomerView)okResult.Value!;

        Assert.Equal(CustomerViews.Customer1.Id, value.Id);
    }

    [Fact]
    public async Task GivenWeAreRetrievingACustomerById_WhenCustomerIsNotFound_ThenShouldReturnNotFound()
    {
        _queryBrokerMoq.Setup(x => x.SendAsync(It.IsAny<GetCustomer>()))
            .ReturnsAsync((Customer)null!);

        var controller = new CustomerQueryController(_queryBrokerMoq.Object, _mapper);

        var actionResult = await controller.GetCustomer(Customers.Customer1.Id);
        var notFoundResult = actionResult as NotFoundResult;

        Assert.NotNull(actionResult);
        Assert.NotNull(notFoundResult);
    }
}
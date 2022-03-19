using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Domain.CustomerAggregate.Queries;
using CustomerService.Domain.CustomerAggregate.QueryHandlers;
using CustomerService.Repositories;
using CustomerService.Tests.Fakes;
using CustomerService.Tests.Helpers;
using Moq;
using Xunit;

namespace CustomerService.Tests.Domain.CustomerAggregate.QueryHandlers;

public class GetCustomersHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ICustomerRepository> _repositoryMock;

    public GetCustomersHandlerTests()
    {
        _repositoryMock = new Mock<ICustomerRepository>();
        _mapper = MappingHelper.GetMapper();
    }

    [Fact]
    public void WhenInstantiated_ThenShouldBeOfCorrectType()
    {
        var handler = new GetCustomersHandler(_repositoryMock.Object);

        Assert.NotNull(handler);
        Assert.IsType<GetCustomersHandler>(handler);
    }

    [Fact]
    public async Task WhenRetrievingEntities_ThenAllEntitiesShouldBeReturned()
    {
        _repositoryMock.Setup(x => x.GetAsync())
            .ReturnsAsync(new List<Customer>
            {
                _mapper.Map<Customer>(Customers.Customer1),
                _mapper.Map<Customer>(Customers.Customer2),
                _mapper.Map<Customer>(Customers.Customer3)
            });

        var handler = new GetCustomersHandler(_repositoryMock.Object);

        var result = await handler.Handle(new GetCustomers(), default);

        Assert.Collection(result,
            c => Assert.Equal(CustomerViews.Customer1.Id, c.Id),
            c => Assert.Equal(CustomerViews.Customer2.Id, c.Id),
            c => Assert.Equal(CustomerViews.Customer3.Id, c.Id));
    }
}
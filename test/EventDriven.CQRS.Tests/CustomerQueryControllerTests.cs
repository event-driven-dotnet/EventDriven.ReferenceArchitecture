using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CustomerService.Controllers;
using CustomerService.Domain.CustomerAggregate.CommandHandlers;
using CustomerService.DTO.Read;
using CustomerService.Mapping;
using EventDriven.CQRS.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace EventDriven.CQRS.Tests
{
    public class CustomerQueryControllerTests
    {
        private readonly IMapper _mapper;

        public CustomerQueryControllerTests()
        {
            _mapper = new MapperConfiguration(
        c => c.AddProfile(new AutoMapperProfile())).CreateMapper();
        }

        [Fact]
        public async Task Get_Retrieves_Customers()
        {
            // Arrange
            var repository = new FakeCustomerRepository();
            var handler = new CustomerCommandHandler(repository, new FakeEventBus(),
                _mapper, new NullLogger<CustomerCommandHandler>());
            var commandController = new CustomerCommandController(handler, _mapper);
            await commandController.Create(Customers.Customer1);
            await commandController.Create(Customers.Customer2);
            await commandController.Create(Customers.Customer3);
            var controller = new CustomerQueryController(repository, _mapper);
            
            // Act
            var actionResult = await controller.GetCustomers();
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var value = (IEnumerable<CustomerView>)okResult.Value;
            Assert.Collection(value,
                c => Assert.Equal(CustomerViews.Customer1.Id, c.Id),
                c => Assert.Equal(CustomerViews.Customer2.Id, c.Id),
                c => Assert.Equal(CustomerViews.Customer3.Id, c.Id));
        }

        [Fact]
        public async Task Get_Retrieves_Customer_By_Id()
        {
            // Arrange
            var repository = new FakeCustomerRepository();
            var handler = new CustomerCommandHandler(repository, new FakeEventBus(),
                _mapper, new NullLogger<CustomerCommandHandler>());
            var commandController = new CustomerCommandController(handler, _mapper);
            await commandController.Create(Customers.Customer1);
            var controller = new CustomerQueryController(repository, _mapper);
            
            // Act
            var actionResult = await controller.GetCustomer(Customers.Customer1.Id);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var value = (CustomerView)okResult.Value;
            Assert.Equal(CustomerViews.Customer1.Id, value.Id);
        }
    }
}

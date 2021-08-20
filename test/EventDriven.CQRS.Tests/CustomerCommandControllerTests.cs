using System;
using System.Threading.Tasks;
using AutoMapper;
using CustomerService.Controllers;
using CustomerService.Domain.CustomerAggregate.CommandHandlers;
using CustomerService.DTO.Write;
using CustomerService.Mapping;
using EventDriven.CQRS.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace EventDriven.CQRS.Tests
{
    public class CustomerCommandControllerTests
    {
        private readonly IMapper _mapper;

        public CustomerCommandControllerTests()
        {
            _mapper = new MapperConfiguration(
        c => c.AddProfile(new AutoMapperProfile())).CreateMapper();
        }

        [Fact]
        public async Task Create_Adds_Customer()
        {
            // Arrange
            var handler = new CustomerCommandHandler(
                new FakeCustomerRepository(), new FakeEventBus(),
                _mapper, new NullLogger<CustomerCommandHandler>());
            var controller = new CustomerCommandController(handler, _mapper);
            
            // Act
            var actionResult = await controller.Create(Customers.Customer1);
            
            // Assert
            var createdResult = Assert.IsType<CreatedResult>(actionResult);
            var value = (Customer)createdResult.Value;
            Assert.Equal(Customers.Customer1.Id, value.Id);
            Assert.NotEqual(default(Guid).ToString(), value.ETag);
        }
        
        [Fact]
        public async Task Update_Updates_Customer()
        {
            // Arrange
            var handler = new CustomerCommandHandler(
                new FakeCustomerRepository(), new FakeEventBus(), 
                _mapper, new NullLogger<CustomerCommandHandler>());
            var controller = new CustomerCommandController(handler, _mapper);
            var customer = (await controller.Create(Customers.Customer1) as CreatedResult)?.Value as Customer;
            customer.ShippingAddress.City = "Los Angeles";
            
            // Act
            var actionResult = await controller.Update(customer);
            
            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(actionResult);
            var value = (Customer)objectResult.Value;
            Assert.Equal(customer.Id, value.Id);
            Assert.NotEqual(customer.ETag, value.ETag);
        }

        [Fact]
        public async Task Remove_Removes_Customer()
        {
            // Arrange
            var repository = new FakeCustomerRepository();
            var handler = new CustomerCommandHandler(
                repository, new FakeEventBus(),
                _mapper, new NullLogger<CustomerCommandHandler>());
            var controller = new CustomerCommandController(handler, _mapper);
            var customer = (await controller.Create(Customers.Customer1) as CreatedResult)?.Value as Customer;
            
            // Act
            var actionResult = await controller.Remove(customer.Id);
            
            // Assert
            Assert.IsType<NoContentResult>(actionResult);
            var value = await repository.Get(customer.Id);
            Assert.Null(value);
        }
    }
}

using System;
using System.Threading.Tasks;
using AutoMapper;
using EventDriven.CQRS.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using OrderService.Controllers;
using OrderService.Domain.OrderAggregate.CommandHandlers;
using OrderService.DTO.Write;
using OrderService.Mapping;
using Xunit;

namespace EventDriven.CQRS.Tests
{
    public class OrderCommandControllerTests
    {
        private readonly IMapper _mapper;
        
        public OrderCommandControllerTests()
        {
            _mapper = new MapperConfiguration(
                c => c.AddProfile(new AutoMapperProfile())).CreateMapper();
        }

        [Fact]
        public async Task Create_Adds_Order()
        {
            // Arrange
            var handler = new OrderCommandHandler(new FakeOrderRepository(),
                new NullLogger<OrderCommandHandler>());
            var controller = new OrderCommandController(handler, _mapper);
            
            // Act
            var actionResult = await controller.Create(Orders.Order1);
            
            // Assert
            var createdResult = Assert.IsType<CreatedResult>(actionResult);
            var value = (Order)createdResult.Value;
            Assert.Equal(Customers.Customer1.Id, value.Id);
            Assert.NotEqual(default(Guid).ToString(), value.ETag);
        }
        
        [Fact]
        public async Task Update_Updates_Order()
        {
            // Arrange
            var handler = new OrderCommandHandler(new FakeOrderRepository(),
                new NullLogger<OrderCommandHandler>());
            var controller = new OrderCommandController(handler, _mapper);
            var order = (await controller.Create(Orders.Order1) as CreatedResult)?.Value as Order;
            order.ShippingAddress.City = "Los Angeles";
            
            // Act
            var actionResult = await controller.Update(order);
            
            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(actionResult);
            var value = (Order)objectResult.Value;
            Assert.Equal(order.Id, value.Id);
            Assert.Equal(order.ShippingAddress.City, value.ShippingAddress.City);
            Assert.NotEqual(order.ETag, value.ETag);
        }
        
        [Fact]
        public async Task Remove_Removes_Order()
        {
            // Arrange
            var repository = new FakeOrderRepository();
            var handler = new OrderCommandHandler(repository,
                new NullLogger<OrderCommandHandler>());
            var controller = new OrderCommandController(handler, _mapper);
            var order = (await controller.Create(Orders.Order1) as CreatedResult)?.Value as Order;
            
            // Act
            var actionResult = await controller.Remove(order.Id);
            
            // Assert
            Assert.IsType<NoContentResult>(actionResult);
            var value = await repository.GetOrder(order.Id);
            Assert.Null(value);
        }
        
        [Fact]
        public async Task Ship_Updates_OrderStatus_To_Shipped()
        {
            // Arrange
            var handler = new OrderCommandHandler(new FakeOrderRepository(),
                new NullLogger<OrderCommandHandler>());
            var controller = new OrderCommandController(handler, _mapper);
            var order = (await controller.Create(Orders.Order1) as CreatedResult)?.Value as Order;
            
            // Act
            var actionResult = await controller.Ship(order.Id, order.ETag);
            
            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(actionResult);
            var value = (Order)objectResult.Value;
            Assert.Equal(Orders.Order1.Id, value.Id);
            Assert.Equal(OrderState.Shipped, value.OrderState);
            Assert.NotEqual(order.ETag, value.ETag);
        }
        
        [Fact]
        public async Task Cancel_Updates_OrderStatus_To_Cancelled()
        {
            // Arrange
            var handler = new OrderCommandHandler(new FakeOrderRepository(),
                new NullLogger<OrderCommandHandler>());
            var controller = new OrderCommandController(handler, _mapper);
            var order = (await controller.Create(Orders.Order1) as CreatedResult)?.Value as Order;
            
            // Act
            var actionResult = await controller.Cancel(order.Id, order.ETag);
            
            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(actionResult);
            var value = (Order)objectResult.Value;
            Assert.Equal(Orders.Order1.Id, value.Id);
            Assert.Equal(OrderState.Cancelled, value.OrderState);
            Assert.NotEqual(order.ETag, value.ETag);
        }
    }
}
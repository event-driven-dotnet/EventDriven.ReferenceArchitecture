using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using OrderService.Controllers;
using OrderService.Domain.OrderAggregate.CommandHandlers;
using OrderService.DTO.Read;
using OrderService.Mapping;
using EventDriven.CQRS.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace EventDriven.CQRS.Tests
{
    public class OrderQueryControllerTests
    {
        private readonly IMapper _mapper;

        public OrderQueryControllerTests()
        {
            _mapper = new MapperConfiguration(
        c => c.AddProfile(new AutoMapperProfile())).CreateMapper();
        }

        [Fact]
        public async Task Get_Retrieves_Orders()
        {
            // Arrange
            var repository = new FakeOrderRepository();
            var handler = new OrderCommandHandler(repository,
                new NullLogger<OrderCommandHandler>());
            var commandController = new OrderCommandController(handler, _mapper);
            await commandController.Create(Orders.Order1);
            await commandController.Create(Orders.Order2);
            var controller = new OrderQueryController(repository);
            
            // Act
            var actionResult = await controller.GetOrders();
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var value = (IEnumerable<OrderView>)okResult.Value;
            Assert.Collection(value,
                c => Assert.Equal(OrderViews.Order1.Id, c.Id),
                c => Assert.Equal(OrderViews.Order2.Id, c.Id));
        }

        [Fact]
        public async Task Get_Retrieves_Order_By_Id()
        {
            // Arrange
            var repository = new FakeOrderRepository();
            var handler = new OrderCommandHandler(repository,
                new NullLogger<OrderCommandHandler>());
            var commandController = new OrderCommandController(handler, _mapper);
            await commandController.Create(Orders.Order1);
            var controller = new OrderQueryController(repository);
            
            // Act
            var actionResult = await controller.GetOrder(Orders.Order1.Id);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var value = (OrderView)okResult.Value;
            Assert.Equal(OrderViews.Order1.Id, value.Id);
        }
    }
}

namespace OrderService.Tests.Controllers {

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using OrderService.Domain.OrderAggregate;
    using DTO.Read;
    using Fakes;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using OrderService.Controllers;
    using OrderService.Repositories;
    using Utils;
    using Xunit;

    public class OrderQueryControllerTests {

        private readonly Mock<IOrderRepository> repositoryMoq;
        private readonly IMapper mapper;

        public OrderQueryControllerTests() {
            repositoryMoq = new Mock<IOrderRepository>();
            mapper = BaseUtils.GetMapper();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType() {
            var controller = new OrderQueryController(repositoryMoq.Object);

            Assert.NotNull(controller);
            Assert.IsAssignableFrom<ControllerBase>(controller);
            Assert.IsType<OrderQueryController>(controller);
        }

        [Fact]
        public async Task WhenRetrievingAllOrders_ThenAllOrdersShouldBeReturned() {
            repositoryMoq.Setup(x => x.GetOrders())
                         .ReturnsAsync(new List<Order> {
                              mapper.Map<Order>(Orders.Order1),
                              mapper.Map<Order>(Orders.Order2)
                          });
            var controller = new OrderQueryController(repositoryMoq.Object);

            var actionResult = await controller.GetOrders();
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var value = (IEnumerable<OrderView>) okResult.Value;
            
            Assert.Collection(value, 
                x => Assert.Equal(OrderViews.Order1.Id, x.Id),
                x => Assert.Equal(OrderViews.Order2.Id, x.Id));
        }

        [Fact]
        public async Task WhenRetrievingAllOrdersForACustomer_ThenAllOrdersShouldBeReturned() {
            repositoryMoq.Setup(x => x.GetCustomerOrders(It.IsAny<Guid>()))
                         .ReturnsAsync(new List<Order> {
                              mapper.Map<Order>(Orders.Order1),
                              mapper.Map<Order>(Orders.Order2)
                          });
            var controller = new OrderQueryController(repositoryMoq.Object);

            var actionResult = await controller.GetOrders(Guid.NewGuid());
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var value = (IEnumerable<OrderView>) okResult.Value;
            
            Assert.Collection(value, 
                x => Assert.Equal(OrderViews.Order1.Id, x.Id),
                x => Assert.Equal(OrderViews.Order2.Id, x.Id));
        }

        [Fact]
        public async Task WhenRetrievingAnOrderById_ThenShouldReturnOrder() {
            repositoryMoq.Setup(x => x.GetOrder(It.IsAny<Guid>()))
                         .ReturnsAsync(mapper.Map<Order>(Orders.Order1));
            var controller = new OrderQueryController(repositoryMoq.Object);

            var actionResult = await controller.GetOrder(Guid.NewGuid());
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var value = (OrderView) okResult.Value;

            Assert.Equal(OrderViews.Order1.Id, value.Id);
        }

    }

}
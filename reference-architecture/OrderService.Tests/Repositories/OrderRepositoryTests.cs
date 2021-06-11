namespace OrderService.Tests.Repositories {

    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using AutoMapper;
    using Fakes;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrderService.Domain.OrderAggregate;
    using OrderService.Repositories;
    using URF.Core.Abstractions;
    using Utils;
    using Xunit;

    public class OrderRepositoryTests {

        private readonly Mock<IDocumentRepository<Order>> documentRepositoryMoq;
        private readonly Mock<ILogger<OrderRepository>> logger;
        private readonly IMapper mapper;
        private readonly Fixture fixture = new Fixture();

        public OrderRepositoryTests() {
            documentRepositoryMoq = new Mock<IDocumentRepository<Order>>();
            logger = new Mock<ILogger<OrderRepository>>();
            mapper = BaseUtils.GetMapper();
        }
        
        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType() {
            var repo = new OrderRepository(documentRepositoryMoq.Object, logger.Object);

            Assert.NotNull(repo);
            Assert.IsAssignableFrom<IOrderRepository>(repo);
            Assert.IsType<OrderRepository>(repo);
        }

        [Fact]
        public async Task GivenWeAreAddingAnOrder_WhenOrderAlreadyExists_ThenShouldReturnNull() {
            var repo = new OrderRepository(documentRepositoryMoq.Object, logger.Object);
            documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(new Order());

            var result = await repo.AddOrder(new Order());

            Assert.Null(result);
        }

        [Fact]
        public async Task WhenAnOrderIsAdded_ThenShouldReturnNewEntity() {
            var order = new Order {SequenceNumber = 0, Id = Guid.NewGuid()};
            var repo = new OrderRepository(documentRepositoryMoq.Object, logger.Object);

            var result = await repo.AddOrder(order);

            Assert.Equal(1, order.SequenceNumber);
        }

        [Fact]
        public async Task GivenWeAreUpdatingAnOrder_WhenOrderDoesNotExist_ThenShouldReturnNull() {
            var repo = new OrderRepository(documentRepositoryMoq.Object, logger.Object);
            documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync((Order) null);

            var result = await repo.UpdateOrder(new Order());

            Assert.Null(result);
        }

        [Fact]
        public async Task GivenWeAreUpdatingAnOrder_WhenETagDoesNotMatch_ThenShouldThrowException() {
            var orderIn = mapper.Map<Order>(Orders.Order1);
            orderIn.ETag = "abc";
            var existing = mapper.Map<Order>(Orders.Order1);
            existing.ETag = "123";
            documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(existing);
            var repo = new OrderRepository(documentRepositoryMoq.Object, logger.Object);

            await Assert.ThrowsAsync<ConcurrencyException>(() => repo.UpdateOrder(orderIn));
        }

        [Fact]
        public async Task WhenOrderIsUpdated_ThenShouldReturnUpdatedEntity() {
            var order = mapper.Map<Order>(Orders.Order1);
            order.SequenceNumber = 1;
            documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(order);
            documentRepositoryMoq.Setup(x => x.FindOneAndReplaceAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(order);
            var repo = new OrderRepository(documentRepositoryMoq.Object, logger.Object);

            var result = await repo.UpdateOrder(order);

            Assert.NotNull(result);
            Assert.Same(order, result);
            Assert.Equal(2, order.SequenceNumber);
        }

        [Fact]
        public async Task GivenWeAreUpdatingAnOrderAddress_WhenOrderDoesNotExist_ThenShouldReturnNull() {
            var repo = new OrderRepository(documentRepositoryMoq.Object, logger.Object);
            documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync((Order) null);

            var result = await repo.UpdateOrderAddress(Guid.Empty, fixture.Create<Address>());

            Assert.Null(result);
        }

        [Fact]
        public async Task WhenOrderAddressIsUpdated_ThenShouldReturnUpdatedEntity() {
            var address = fixture.Create<Address>();
            var existingOrder = fixture.Create<Order>();
            documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(existingOrder);
            documentRepositoryMoq.Setup(x => x.FindOneAndReplaceAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(existingOrder);
            var repo = new OrderRepository(documentRepositoryMoq.Object, logger.Object);

            var result = await repo.UpdateOrderAddress(Guid.Empty, address);

            Assert.NotNull(result);
            Assert.Equal(address, result.ShippingAddress);
        }

        [Fact]
        public async Task GivenWeAreUpdatingOrderState_WhenOrderDoesNotExist_ThenShouldReturnNull() {
            documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync((Order) null);
            var repo = new OrderRepository(documentRepositoryMoq.Object, logger.Object);

            var result = await repo.UpdateOrderState(new Order(), OrderState.Shipped);

            Assert.Null(result);
        }

        [Fact]
        public async Task GivenWeAreUpdatingOrderState_WhenETagDoesNotMatch_ThenShouldThrowException() {
            var orderIn = mapper.Map<Order>(Orders.Order1);
            orderIn.ETag = "abc";
            var existing = mapper.Map<Order>(Orders.Order1);
            existing.ETag = "123";
            documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(existing);
            var repo = new OrderRepository(documentRepositoryMoq.Object, logger.Object);

            await Assert.ThrowsAsync<ConcurrencyException>(() => repo.UpdateOrderState(orderIn, OrderState.Shipped));
        }

        [Fact]
        public async Task WhenOrderStateIsUpdated_ThenShouldReturnUpdatedEntity() {
            var order = mapper.Map<Order>(Orders.Order1);
            order.SequenceNumber = 1;
            documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(order);
            documentRepositoryMoq.Setup(x => x.FindOneAndReplaceAsync(It.IsAny<Expression<Func<Order, bool>>>(), It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(order);
            var repo = new OrderRepository(documentRepositoryMoq.Object, logger.Object);

            var result = await repo.UpdateOrderState(order, OrderState.Shipped);

            Assert.NotNull(result);
            Assert.Same(order, result);
            Assert.Equal(2, order.SequenceNumber);
            Assert.Equal(OrderState.Shipped, result.OrderState);
        }

    }

}
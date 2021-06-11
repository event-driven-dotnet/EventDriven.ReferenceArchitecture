namespace OrderService.Tests.Integration.EventHandlers {

    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using AutoMapper;
    using Common.Integration.Events;
    using EventDriven.EventBus.Abstractions;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrderService.Domain.OrderAggregate;
    using OrderService.Integration.EventHandlers;
    using OrderService.Repositories;
    using Utils;
    using Xunit;

    public class CustomerAddressUpdatedEventHandlerTests {

        private readonly Mock<IOrderRepository> repositoryMoq;
        private readonly IMapper mapper;
        private readonly Mock<ILogger<CustomerAddressUpdatedEventHandler>> logger;
        private readonly Fixture fixture = new Fixture();

        public CustomerAddressUpdatedEventHandlerTests() {
            repositoryMoq = new Mock<IOrderRepository>();
            mapper = BaseUtils.GetMapper();
            logger = new Mock<ILogger<CustomerAddressUpdatedEventHandler>>();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType() {
            var handler = new CustomerAddressUpdatedEventHandler(repositoryMoq.Object, mapper, logger.Object);

            Assert.NotNull(handler);
            Assert.IsAssignableFrom<IntegrationEventHandler<CustomerAddressUpdated>>(handler);
            Assert.IsType<CustomerAddressUpdatedEventHandler>(handler);
        }

        [Fact]
        public async Task WhenEventIsHandled_ThenOrderAddressShouldGetUpdated() {
            var address = fixture.Create<Common.Integration.Models.Address>();
            var updatedEvent = new CustomerAddressUpdated(Guid.NewGuid(), address);
            var addressWasUpdated = false;
            repositoryMoq.Setup(x => x.GetCustomerOrders(It.IsAny<Guid>()))
                         .ReturnsAsync(new[] {fixture.Create<Order>()});
            repositoryMoq.Setup(x => x.UpdateOrderAddress(It.IsAny<Guid>(), It.IsAny<Address>()))
                         .Callback<Guid, Address>((o, a) => { addressWasUpdated = true; });
            var handler = new CustomerAddressUpdatedEventHandler(repositoryMoq.Object, mapper, logger.Object);

            await handler.HandleAsync(updatedEvent);

            Assert.True(addressWasUpdated);
        }

    }

}
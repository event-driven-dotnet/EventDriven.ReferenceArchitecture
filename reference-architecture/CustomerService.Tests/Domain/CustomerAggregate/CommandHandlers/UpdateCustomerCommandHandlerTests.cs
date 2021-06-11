namespace CustomerService.Tests.Domain.CustomerAggregate.CommandHandlers {

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using AutoMapper;
    using Common.Integration.Events;
    using CustomerService.Domain.CustomerAggregate;
    using CustomerService.Domain.CustomerAggregate.CommandHandlers;
    using CustomerService.Domain.CustomerAggregate.Commands;
    using CustomerService.Repositories;
    using EventDriven.CQRS.Abstractions.Commands;
    using EventDriven.EventBus.Abstractions;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Utils;
    using Xunit;

    public class UpdateCustomerCommandHandlerTests {

        private readonly Mock<ILogger<UpdateCustomerCommandHandler>> loggerMoq;
        private readonly Mock<ICustomerRepository> repositoryMoq;
        private readonly IMapper mapper = BaseUtils.GetMapper();
        private readonly Mock<IEventBus> eventBusMoq;
        private readonly Fixture fixture = new();

        public UpdateCustomerCommandHandlerTests() {
            loggerMoq = new Mock<ILogger<UpdateCustomerCommandHandler>>();
            repositoryMoq = new Mock<ICustomerRepository>();
            eventBusMoq = new Mock<IEventBus>();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType() {
            var handler = GetHandler();

            Assert.NotNull(handler);
            Assert.IsType<UpdateCustomerCommandHandler>(handler);
        }

        [Fact]
        public async Task WhenNoExistingCustomerIsFound_ThenShouldReturnNotFound() {
            repositoryMoq.Setup(x => x.Get(It.IsAny<Guid>()))
                         .ReturnsAsync(GenerateCustomer());
            
            var handler = GetHandler();

            var cmdResult = await handler.Handle(new UpdateCustomer(GenerateCustomer()), CancellationToken.None);

            Assert.Equal(CommandOutcome.NotFound, cmdResult.Outcome);
        }

        [Fact]
        public async Task WhenTheAddressIsUpdated_ThenEventShouldBePublished() {
            var existingCustomer = GenerateCustomer();
            var updatedCustomer = GenerateCustomer();
            repositoryMoq.Setup(x => x.Get(It.IsAny<Guid>()))
                         .ReturnsAsync(existingCustomer);
            repositoryMoq.Setup(x => x.Update(It.IsAny<Customer>()))
                         .ReturnsAsync(updatedCustomer);
            var eventRaised = false;
            eventBusMoq.Setup(x => x.PublishAsync(It.IsAny<CustomerAddressUpdated>(), It.IsAny<string>(), It.IsAny<string>()))
                       .Callback(() => eventRaised = true);
            var handler = GetHandler();

            await handler.Handle(new UpdateCustomer(updatedCustomer), CancellationToken.None);

            Assert.True(eventRaised);
        }
        
        [Fact]
        public async Task WhenTheAddressIsNotUpdated_ThenEventShouldNotBePublished() {
            var existingCustomer = GenerateCustomer();
            repositoryMoq.Setup(x => x.Get(It.IsAny<Guid>()))
                         .ReturnsAsync(existingCustomer);
            repositoryMoq.Setup(x => x.Update(It.IsAny<Customer>()))
                         .ReturnsAsync(existingCustomer);
            var eventRaised = false;
            eventBusMoq.Setup(x => x.PublishAsync(It.IsAny<CustomerAddressUpdated>(), It.IsAny<string>(), It.IsAny<string>()))
                       .Callback(() => eventRaised = true);
            var handler = GetHandler();

            await handler.Handle(new UpdateCustomer(existingCustomer), CancellationToken.None);

            Assert.False(eventRaised);
        }
        
        [Fact]
        public async Task WhenTheCustomerIsUpdated_ThenShouldReturnSuccess() {
            var existingCustomer = GenerateCustomer();
            repositoryMoq.Setup(x => x.Get(It.IsAny<Guid>()))
                         .ReturnsAsync(existingCustomer);
            repositoryMoq.Setup(x => x.Update(It.IsAny<Customer>()))
                         .ReturnsAsync(existingCustomer);
            var handler = GetHandler();

            var cmdResult = await handler.Handle(new UpdateCustomer(existingCustomer), CancellationToken.None);

            Assert.Equal(CommandOutcome.Accepted, cmdResult.Outcome);
        }

        [Fact]
        public async Task WhenConcurrencyExceptionOccurs_ThenShouldReturnConflict() {
            var existingCustomer = GenerateCustomer();
            repositoryMoq.Setup(x => x.Get(It.IsAny<Guid>()))
                         .ReturnsAsync(existingCustomer);
            repositoryMoq.Setup(x => x.Update(It.IsAny<Customer>()))
                         .ThrowsAsync(new ConcurrencyException());
            var handler = GetHandler();

            var cmdResult = await handler.Handle(new UpdateCustomer(existingCustomer), CancellationToken.None);

            Assert.Equal(CommandOutcome.Conflict, cmdResult.Outcome);
        }

        private UpdateCustomerCommandHandler GetHandler() =>
            new(loggerMoq.Object,
                repositoryMoq.Object,
                mapper,
                eventBusMoq.Object);
        
        private Customer GenerateCustomer() =>  
            fixture.Build<Customer>()
                   .With(x => x.ShippingAddress)
                   .With(x => x.Id)
                   .Create();

    }

}
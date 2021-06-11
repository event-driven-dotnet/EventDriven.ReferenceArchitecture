namespace CustomerService.Tests.Repositories {

    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using CustomerService.Domain.CustomerAggregate;
    using CustomerService.Repositories;
    using Fakes;
    using Microsoft.Extensions.Logging;
    using Moq;
    using URF.Core.Abstractions;
    using Utils;
    using Xunit;

    public class CustomerRepositoryTests {

        private readonly Mock<IDocumentRepository<Customer>> documentRepositoryMoq;
        private readonly Mock<ILogger<CustomerRepository>> logger;
        private readonly Fixture fixture = new();

        public CustomerRepositoryTests() {
            documentRepositoryMoq = new Mock<IDocumentRepository<Customer>>();
            logger = new Mock<ILogger<CustomerRepository>>();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType() {
            var repo = new CustomerRepository(documentRepositoryMoq.Object, logger.Object);

            Assert.NotNull(repo);
            Assert.IsAssignableFrom<ICustomerRepository>(repo);
            Assert.IsType<CustomerRepository>(repo);
        }

        [Fact]
        public async Task GivenWeAreSearchingById_WhenCustomerIsNotFound_ThenShouldReturnNull() {
            var repo = new CustomerRepository(documentRepositoryMoq.Object, logger.Object);

            var customer = await repo.Get(Guid.NewGuid());

            Assert.Null(customer);
        }
        
        [Fact]
        public async Task GivenWeAreSearchingById_WhenCustomerIsFound_ThenShouldReturnCustomer() {
            documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(fixture.Build<Customer>().Create());
            var repo = new CustomerRepository(documentRepositoryMoq.Object, logger.Object);

            var customer = await repo.Get(Guid.NewGuid());

            Assert.NotNull(customer);
        }
        
        [Fact]
        public async Task GivenWeAreSearchingAllCustomers_WhenCustomersAreFound_ThenShouldReturnCollection() {
            var customerCollection = new List<Customer> {
                                                            fixture.Create<Customer>(),
                                                            fixture.Create<Customer>(),
                                                            fixture.Create<Customer>(),
                                                        };
            documentRepositoryMoq.Setup(x => x.FindManyAsync(It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(customerCollection);
            var repo = new CustomerRepository(documentRepositoryMoq.Object, logger.Object);

            var customersResult = await repo.Get();

            Assert.Collection(customersResult,
                              x => Assert.Equal(customerCollection[0].Id, x.Id),
                              x => Assert.Equal(customerCollection[1].Id, x.Id),
                              x => Assert.Equal(customerCollection[2].Id, x.Id));
        }

        [Fact]
        public async Task WhenCustomerIsRemoved_ThenShouldReturnNumberOfRecordsAffected() {
            documentRepositoryMoq.Setup(x => x.DeleteOneAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(1);
            
            var repo = new CustomerRepository(documentRepositoryMoq.Object, logger.Object);

            var result = await repo.Remove(Guid.NewGuid());

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GivenWeAreUpdatingACustomer_WhenTheCustomerDoesNotExist_ThenShouldReturnNull() {
            documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync((Customer) null);
            
            var repo = new CustomerRepository(documentRepositoryMoq.Object, logger.Object);

            var result = await repo.Update(fixture.Build<Customer>().Create());

            Assert.Null(result);
        }

        [Fact]
        public async Task GivenWeAreUpdatingACustomer_WhenTheETagValueDoesNotMatchTheLatest_ThenShouldThrowConcurrencyError() {
            var existingCustomer = fixture.Build<Customer>().Create();
            documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(existingCustomer);
            var repo = new CustomerRepository(documentRepositoryMoq.Object, logger.Object);

            await Assert.ThrowsAsync<ConcurrencyException>(() => repo.Update(fixture.Build<Customer>().Create()));
        }
        
        [Fact]
        public async Task WhenTheCustomerIsUpdated_ThenShouldReturnUpdatedEntity() {
            var customer = fixture.Build<Customer>().Create();
            var originalSequenceNumber = customer.SequenceNumber;
            documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(customer);
            documentRepositoryMoq.Setup(x => x.FindOneAndReplaceAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(customer);
            var repo = new CustomerRepository(documentRepositoryMoq.Object, logger.Object);

            var result = await repo.Update(customer);

            Assert.NotNull(result);
            Assert.Equal(originalSequenceNumber + 1, result.SequenceNumber);
        }

        [Fact]
        public async Task GivenWeAreAddingACustomer_WhenTheCustomerAlreadyExists_ThenShouldReturnNull() {
            documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(fixture.Build<Customer>().Create());
            
            var repo = new CustomerRepository(documentRepositoryMoq.Object, logger.Object);

            var result = await repo.Add(fixture.Build<Customer>().Create());

            Assert.Null(result);
        }
        
        [Fact]
        public async Task WhenTheCustomerIsAdded_ThenShouldReturnEntity() {
            var customer = fixture.Build<Customer>().Create();
            documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync((Customer) null);
            documentRepositoryMoq.Setup(x => x.InsertOneAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(customer);
            var repo = new CustomerRepository(documentRepositoryMoq.Object, logger.Object);

            var result = await repo.Add(customer);

            Assert.NotNull(result);
            Assert.Equal(customer.Id, result.Id);
        }

    }

}
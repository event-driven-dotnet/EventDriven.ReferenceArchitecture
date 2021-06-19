using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using URF.Core.Abstractions;
using Xunit;

namespace CustomerService.Tests.Repositories
{

    public class CustomerRepositoryTests
    {

        private readonly Mock<IDocumentRepository<Customer>> _documentRepositoryMoq;
        private readonly Fixture _fixture = new();
        private readonly Mock<ILogger<CustomerRepository>> _logger;

        public CustomerRepositoryTests()
        {
            _documentRepositoryMoq = new Mock<IDocumentRepository<Customer>>();
            _logger = new Mock<ILogger<CustomerRepository>>();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType()
        {
            var repo = new CustomerRepository(_documentRepositoryMoq.Object, _logger.Object);

            Assert.NotNull(repo);
            Assert.IsAssignableFrom<ICustomerRepository>(repo);
            Assert.IsType<CustomerRepository>(repo);
        }

        [Fact]
        public async Task GivenWeAreSearchingById_WhenCustomerIsNotFound_ThenShouldReturnNull()
        {
            var repo = new CustomerRepository(_documentRepositoryMoq.Object, _logger.Object);

            var customer = await repo.Get(Guid.NewGuid());

            Assert.Null(customer);
        }

        [Fact]
        public async Task GivenWeAreSearchingById_WhenCustomerIsFound_ThenShouldReturnCustomer()
        {
            _documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(_fixture.Build<Customer>()
                                                        .Create());
            var repo = new CustomerRepository(_documentRepositoryMoq.Object, _logger.Object);

            var customer = await repo.Get(Guid.NewGuid());

            Assert.NotNull(customer);
        }

        [Fact]
        public async Task GivenWeAreSearchingAllCustomers_WhenCustomersAreFound_ThenShouldReturnCollection()
        {
            var customerCollection = new List<Customer>
            {
                _fixture.Create<Customer>(),
                _fixture.Create<Customer>(),
                _fixture.Create<Customer>()
            };
            _documentRepositoryMoq.Setup(x => x.FindManyAsync(It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(customerCollection);
            var repo = new CustomerRepository(_documentRepositoryMoq.Object, _logger.Object);

            var customersResult = await repo.Get();

            Assert.Collection(customersResult,
                x => Assert.Equal(customerCollection[0]
                       .Id,
                    x.Id),
                x => Assert.Equal(customerCollection[1]
                       .Id,
                    x.Id),
                x => Assert.Equal(customerCollection[2]
                       .Id,
                    x.Id));
        }

        [Fact]
        public async Task WhenCustomerIsRemoved_ThenShouldReturnNumberOfRecordsAffected()
        {
            _documentRepositoryMoq.Setup(x => x.DeleteOneAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(1);

            var repo = new CustomerRepository(_documentRepositoryMoq.Object, _logger.Object);

            var result = await repo.Remove(Guid.NewGuid());

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GivenWeAreUpdatingACustomer_WhenTheCustomerDoesNotExist_ThenShouldReturnNull()
        {
            _documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync((Customer) null);

            var repo = new CustomerRepository(_documentRepositoryMoq.Object, _logger.Object);

            var result = await repo.Update(_fixture.Build<Customer>()
                                                   .Create());

            Assert.Null(result);
        }

        [Fact]
        public async Task GivenWeAreUpdatingACustomer_WhenTheETagValueDoesNotMatchTheLatest_ThenShouldThrowConcurrencyError()
        {
            var existingCustomer = _fixture.Build<Customer>()
                                           .Create();
            _documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(existingCustomer);
            var repo = new CustomerRepository(_documentRepositoryMoq.Object, _logger.Object);

            await Assert.ThrowsAsync<ConcurrencyException>(() => repo.Update(_fixture.Build<Customer>()
                                                                                     .Create()));
        }

        [Fact]
        public async Task WhenTheCustomerIsUpdated_ThenShouldReturnUpdatedEntity()
        {
            var customer = _fixture.Build<Customer>()
                                   .Create();
            var originalSequenceNumber = customer.SequenceNumber;
            _documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(customer);
            _documentRepositoryMoq
               .Setup(x => x.FindOneAndReplaceAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(customer);
            var repo = new CustomerRepository(_documentRepositoryMoq.Object, _logger.Object);

            var result = await repo.Update(customer);

            Assert.NotNull(result);
            Assert.Equal(originalSequenceNumber + 1, result.SequenceNumber);
        }

        [Fact]
        public async Task GivenWeAreAddingACustomer_WhenTheCustomerAlreadyExists_ThenShouldReturnNull()
        {
            _documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(_fixture.Build<Customer>()
                                                        .Create());

            var repo = new CustomerRepository(_documentRepositoryMoq.Object, _logger.Object);

            var result = await repo.Add(_fixture.Build<Customer>()
                                                .Create());

            Assert.Null(result);
        }

        [Fact]
        public async Task WhenTheCustomerIsAdded_ThenShouldReturnEntity()
        {
            var customer = _fixture.Build<Customer>()
                                   .Create();
            _documentRepositoryMoq.Setup(x => x.FindOneAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync((Customer) null);
            _documentRepositoryMoq.Setup(x => x.InsertOneAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(customer);
            var repo = new CustomerRepository(_documentRepositoryMoq.Object, _logger.Object);

            var result = await repo.Add(customer);

            Assert.NotNull(result);
            Assert.Equal(customer.Id, result.Id);
        }

    }

}
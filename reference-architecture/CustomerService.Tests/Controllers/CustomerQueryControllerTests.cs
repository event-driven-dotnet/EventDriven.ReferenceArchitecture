namespace CustomerService.Tests.Controllers {

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using CustomerService.Controllers;
    using CustomerService.Domain.CustomerAggregate;
    using CustomerService.Repositories;
    using DTO.Read;
    using Fakes;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Utils;
    using Xunit;

    public class CustomerQueryControllerTests {

        private readonly IMapper mapper;
        private readonly Mock<ICustomerRepository> repositoryMoq;

        public CustomerQueryControllerTests() {
            mapper = BaseUtils.GetMapper();
            repositoryMoq = new Mock<ICustomerRepository>();
        }

        [Fact]
        public void WhenInstantiated_ThenShouldBeOfCorrectType() {
            var controller = new CustomerQueryController(repositoryMoq.Object, mapper);

            Assert.IsAssignableFrom<ControllerBase>(controller);
            Assert.IsType<CustomerQueryController>(controller);
        }

        [Fact]
        public async Task WhenRetrievingAllCustomers_ThenAllCustomersShouldBeReturned() {
            repositoryMoq.Setup(x => x.Get())
                         .ReturnsAsync(new List<Customer> {
                              mapper.Map<Customer>(Customers.Customer1), 
                              mapper.Map<Customer>(Customers.Customer2), 
                              mapper.Map<Customer>(Customers.Customer3)
                          });
            var controller = new CustomerQueryController(repositoryMoq.Object, mapper);

            var actionResult = await controller.GetCustomers();
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var value = (IEnumerable<CustomerView>) okResult.Value;

            Assert.Collection(value,
                c => Assert.Equal(CustomerViews.Customer1.Id, c.Id),
                c => Assert.Equal(CustomerViews.Customer2.Id, c.Id),
                c => Assert.Equal(CustomerViews.Customer3.Id, c.Id));
        }

        [Fact]
        public async Task GivenWeAreRetrievingACustomerById_WhenSuccessful_ThenCorrectCustomerShouldBeReturned() {
            repositoryMoq.Setup(x => x.Get(It.Is<Guid>(guid => guid == Customers.Customer1.Id)))
                         .ReturnsAsync(mapper.Map<Customer>(Customers.Customer1));

            var controller = new CustomerQueryController(repositoryMoq.Object, mapper);

            var actionResult = await controller.GetCustomer(Customers.Customer1.Id);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var value = (CustomerView) okResult.Value;

            Assert.Equal(CustomerViews.Customer1.Id, value.Id);
        }

        [Fact]
        public async Task GivenWeAreRetrievingACustomerById_WhenCustomerIsNotFound_ThenShouldReturnNotFound() {
            repositoryMoq.Setup(x => x.Get(It.IsAny<Guid>()))
                         .ReturnsAsync((Customer) null);

            var controller = new CustomerQueryController(repositoryMoq.Object, mapper);

            var actionResult = await controller.GetCustomer(Customers.Customer1.Id);
            var notFoundResult = actionResult as NotFoundResult;

            Assert.NotNull(actionResult);
            Assert.NotNull(notFoundResult);
        }

    }

}
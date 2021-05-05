using FluentAssertions;
using GroceryStore.Domain;
using GroceryStoreApi.Infrastructure;
using GroceryStoreApi.IntegrationTests;
using GroceryStoreApi.IntegrationTests.Mock;
using GroceryStoreAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GroceryStoreApi.UnitTests.Controllers
{
    public class CustomerControllerTests : ServiceTestBase
    {
        private readonly Mock<IHttpContextAccessor> httpContextAccessorMock;

        public CustomerControllerTests()
        {
            this.httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            this.httpContextAccessorMock
                .Setup(m => m.HttpContext)
                .Returns(() => new MockHttpContext(ServiceProvider));
        }

        [Fact]
        public async Task get_should_return_list_on_success()
        {
            // arrange
            base.SetupEnvironment((services) =>
            {
                services.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), this.httpContextAccessorMock.Object));
            });

            var controller = base.ServiceProvider.GetService<CustomerController>();

            // act
            var response = await controller.Get().ConfigureAwait(false);

            // assert
            response.Should().NotBeNull();
            var okResult = response as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var actualCustomers = okResult.Value as IEnumerable<Customer>;
            actualCustomers.Should().NotBeNull();
            actualCustomers.Should().HaveCountGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task get_by_id_should_return_customer_details_on_success()
        {
            // arrange
            base.SetupEnvironment((services) =>
            {
                services.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), this.httpContextAccessorMock.Object));
            });

            var controller = base.ServiceProvider.GetService<CustomerController>();

            // act
            var response = await controller.Get(1).ConfigureAwait(false);

            // assert
            var okResult = response as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var actualCustomer = okResult.Value as Customer;
            actualCustomer.Should().NotBeNull();
            actualCustomer.Id.Should().Be(1);
            actualCustomer.Name.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task get_by_id_should_return_error_when_id_is_invalid(int id)
        {
            // arrange
            base.SetupEnvironment((services) =>
            {
                services.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), this.httpContextAccessorMock.Object));
            });

            var controller = base.ServiceProvider.GetService<CustomerController>();

            // act
            var response = await controller.Get(id).ConfigureAwait(false);

            // assert
            var badRequestResult = response as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            var validationException = badRequestResult.Value as IEnumerable<ValidationResult>;
            validationException.Should().NotBeNull();
            validationException.Should().HaveCount(1);
            validationException.First().Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task create_customer_should_return_success_when_valid_request()
        {
            // arrange
            base.SetupEnvironment((services) =>
            {
                services.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), this.httpContextAccessorMock.Object));
            });

            var controller = base.ServiceProvider.GetService<CustomerController>();
            var newCustomer = new Customer
            {
                Name = "Test-Customer" + Guid.NewGuid().ToString()
            };

            // act
            var response = await controller.Create(newCustomer).ConfigureAwait(false);

            // assert
            var okResult = response as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            var actualCustomer = okResult.Value as Customer;
            actualCustomer.Should().NotBeNull();
            actualCustomer.Id.Should().NotBe(0);
            actualCustomer.Name.Should().NotBeNullOrWhiteSpace();
            actualCustomer.Name.Should().Be(newCustomer.Name);
        }

        [Fact]
        public async Task create_customer_should_return_error_when_invalid_request()
        {
            // arrange
            base.SetupEnvironment((services) =>
            {
                services.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), this.httpContextAccessorMock.Object));
            });

            var controller = base.ServiceProvider.GetService<CustomerController>();
            var newCustomer = new Customer
            {
                Name = ""
            };

            // act
            var response = await controller.Create(newCustomer).ConfigureAwait(false);

            // assert
            var badRequestResult = response as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            var validationException = badRequestResult.Value as IEnumerable<ValidationResult>;
            validationException.Should().NotBeNull();
            validationException.Should().HaveCount(1);
            validationException.First().Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task create_customer_should_return_error_when_customer_already_exists()
        {
            // arrange
            base.SetupEnvironment((services) =>
            {
                services.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), this.httpContextAccessorMock.Object));
            });

            var controller = base.ServiceProvider.GetService<CustomerController>();
            var newCustomer = new Customer
            {
                Name = "Bob"
            };

            // act
            var response = await controller.Create(newCustomer).ConfigureAwait(false);

            // assert
            var badRequestResult = response as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            var validationException = badRequestResult.Value as ValidationResult;
            validationException.Should().NotBeNull();
            validationException.Message.Should().NotBeNullOrWhiteSpace();
            validationException.Message.Should().Be(Constants.CustomerAlreadyExists);
        }

        [Fact]
        public async Task update_customer_should_return_success_when_valid_request()
        {
            // arrange
            base.SetupEnvironment((services) =>
            {
                services.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), this.httpContextAccessorMock.Object));
            });

            var controller = base.ServiceProvider.GetService<CustomerController>();
            var updateCustomer = new Customer
            {
                Id = 3,
                Name = "Test-Customer" + Guid.NewGuid().ToString()
            };

            // act
            var response = await controller.Update(updateCustomer).ConfigureAwait(false);

            // assert
            var okResult = response as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task update_customer_should_return_error_when_invalid_request(string name)
        {
            // arrange
            base.SetupEnvironment((services) =>
            {
                services.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), this.httpContextAccessorMock.Object));
            });

            var controller = base.ServiceProvider.GetService<CustomerController>();
            var newCustomer = new Customer
            {
                Name = name
            };

            // act
            var response = await controller.Create(newCustomer).ConfigureAwait(false);

            // assert
            var badRequestResult = response as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            var validationException = badRequestResult.Value as IEnumerable<ValidationResult>;
            validationException.Should().NotBeNull();
            validationException.Should().HaveCount(1);
            validationException.First().Message.Should().NotBeNullOrWhiteSpace();
            validationException.First().Message.Should().Be(Constants.CustomerNameRequiredMessage);
        }

        [Fact]
        public async Task delete_customer_should_return_success_when_valid_request()
        {
            // arrange
            base.SetupEnvironment((services) =>
            {
                services.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), this.httpContextAccessorMock.Object));
            });

            var controller = base.ServiceProvider.GetService<CustomerController>();

            // act
            var getResponse = await controller.Get().ConfigureAwait(false);
            var okResult = getResponse as OkObjectResult;
            var actualCustomers = okResult.Value as IEnumerable<Customer>;
            int customerId = actualCustomers.Max(c => c.Id);
            var deleteResponse = await controller.Delete(customerId).ConfigureAwait(false);

            // assert
            okResult = deleteResponse as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task delete_customer_should_return_error_when_invalid_id_is_passed(int id)
        {
            // arrange
            base.SetupEnvironment((services) =>
            {
                services.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), this.httpContextAccessorMock.Object));
            });

            var controller = base.ServiceProvider.GetService<CustomerController>();
           
            // act
            var response = await controller.Delete(id).ConfigureAwait(false);

            // assert
            var badRequestResult = response as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            var validationException = badRequestResult.Value as IEnumerable<ValidationResult>;
            validationException.Should().NotBeNull();
            validationException.Should().HaveCount(1);
            validationException.First().Message.Should().NotBeNullOrWhiteSpace();
            validationException.First().Message.Should().Be(Constants.CustomerIdRequiredMessage);
        }

        [Fact]
        public async Task delete_customer_should_return_not_found_error()
        {
            // arrange
            base.SetupEnvironment((services) =>
            {
                services.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), this.httpContextAccessorMock.Object));
            });

            var controller = base.ServiceProvider.GetService<CustomerController>();

            // act
            var response = await controller.Delete(444444).ConfigureAwait(false);

            // assert
            var notFoundResult = response as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            var validationException = notFoundResult.Value as ValidationResult;
            validationException.Should().NotBeNull();
            validationException.Message.Should().NotBeNullOrWhiteSpace();
            validationException.Message.Should().Be(Constants.CustomerNotFoundMessage);
        }
    }
}

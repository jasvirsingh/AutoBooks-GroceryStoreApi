using FluentAssertions;
using GroceryStore.Data.Access.Interfaces;
using GroceryStore.Domain;
using GroceryStoreApi.UnitTests.Mock;
using GroceryStoreAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
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
            response.Should().HaveCountGreaterOrEqualTo(1);
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
            response.Should().NotBeNull();
           
        }
    }
}

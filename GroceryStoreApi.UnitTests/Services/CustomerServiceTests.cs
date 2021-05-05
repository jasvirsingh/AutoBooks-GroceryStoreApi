using FluentAssertions;
using GroceryStore.Data;
using GroceryStore.Data.Access.Interfaces;
using GroceryStore.Data.Access.Repo;
using GroceryStore.Domain;
using GroceryStore.Services;
using GroceryStoreApi.Infrastructure;
using GroceryStoreApi.Infrastructure.Exceptions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GroceryStoreApi.UnitTest
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> mockCustomerRepository;
        
        public CustomerServiceTests()
        {
            mockCustomerRepository = new Mock<ICustomerRepository>();
        }

        [Fact]
        public async Task get_all_customers()
        {
            // arrange
            var customerService = new CustomerService(mockCustomerRepository.Object);

            var fakeResponse = new List<CustomerEntity>
            {
                new CustomerEntity{ Id = 1, Name ="Tom"},
                new CustomerEntity{ Id = 2, Name ="Smith"},
                new CustomerEntity{ Id = 3, Name ="Richard"}
            };

            mockCustomerRepository.Setup(c => c.GetAll())
                .Returns(Task.FromResult(fakeResponse));

            // act
            var result = await customerService.GetAll();

            // assert
            result.Should().NotBeNull();
            result.Count.Should().Be(fakeResponse.Count);
            for (int i = 0; i < fakeResponse.Count; i++)
            {
                result[i].Id.Should().Be(fakeResponse[i].Id);
                result[i].Name.Should().Be(fakeResponse[i].Name);
            }

        }

        [Fact]
        public async Task get_by_id_should_return_customer_details_if_customer_id_exists()
        {
            // arrange
            var customerService = new CustomerService(mockCustomerRepository.Object);
            var id = 3;

            var fakeResponse = new CustomerEntity
            {
                 Id = id, 
                Name ="Richard"
            };

            mockCustomerRepository.Setup(c => c.GetById(id))
                .Returns(Task.FromResult(fakeResponse));

            // act
            var result = await customerService.GetById(id);

            // assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            result.Name.Should().Be(fakeResponse.Name);
        }

        [Xunit.Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task get_by_id_should_returns_bad_request_if_customer_id_is_invalid(int id)
        {
            // arrange
            var customerService = new CustomerService(mockCustomerRepository.Object);
            
            // act
            var exception = await Xunit.Assert.ThrowsAsync<RqValidationFailedException>(async ()=> await customerService.GetById(id));

            // assert
            exception.Should().NotBeNull();
            exception.Messages.Should().NotBeNull();
            exception.Messages.Count.Should().Be(1);
            exception.Messages[0].Message.Should().Be(Constants.CustomerIdRequiredMessage);
        }

        [Xunit.Theory]
        [InlineData("Tom")]
        public async Task create_customer_should_return_success(string name)
        {
            // arrange
            var customerService = new CustomerService(mockCustomerRepository.Object);

            mockCustomerRepository.Setup(c => c.Add(It.IsAny<CustomerEntity>()))
                .Returns(Task.FromResult(new CustomerEntity { Id = 4, Name = name }));

            // act
            var result = await customerService.Add(new Customer { Name = name });

            // assert
            result.Should().NotBeNull();
            result.Id.Should().Be(4);
            result.Name.Should().Be(name);
        }

        [Xunit.Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task create_customer_should_returns_bad_request_if_invalid_request(string name)
        {
            // arrange
            var customerService = new CustomerService(mockCustomerRepository.Object);

            var newCustomer = new Customer { Name = name };
            // act
            var exception = await Xunit.Assert.ThrowsAsync<RqValidationFailedException>(async () => await customerService.Add(newCustomer));

            // assert
            exception.Should().NotBeNull();
            exception.Messages.Should().NotBeNull();
            exception.Messages.Count.Should().Be(1);
            exception.Messages[0].Message.Should().Be(Constants.CustomerNameRequiredMessage);
        }

        [Xunit.Theory]
        [InlineData("Bob")]
        public async Task create_customer_should_returns_duplicate_exception_if_customer_already_exists(string name)
        {
            // arrange
            var customerService = new CustomerService(new CustomerJsonRepository());

            var newCustomer = new Customer { Name = name };
            // act
            var exception = await Xunit.Assert.ThrowsAsync<DuplicateCustomerException>(async () => await customerService.Add(newCustomer));

            // assert
            exception.Should().NotBeNull();
            exception.Message.Should().NotBeNull();
            exception.Message.Should().Be(Constants.CustomerAlreadyExists);
        }


        [Xunit.Theory]
        [InlineData(1,"Tom2")]
        public async Task update_customer_should_return_success(int id, string name)
        {
            // arrange
            var customerService = new CustomerService(mockCustomerRepository.Object);

            mockCustomerRepository.Setup(c => c.Update(It.IsAny<CustomerEntity>()))
                .Verifiable();

            // act
             await customerService.Update(new Customer{ Name = name, Id = id });

            // assert
           Xunit.Assert.True(true);
        }

        [Xunit.Theory]
        [InlineData(0, "test")]
        [InlineData(-1, "test")]
        [InlineData(1, "")]

        public async Task update_customer_should_throw_exception_if_invalid_request(int id, string name)
        {
            // arrange
            var customerService = new CustomerService(mockCustomerRepository.Object);

            var customer = new Customer { Id = id,  Name = name };
            // act
            var exception = await Xunit.Assert.ThrowsAsync<RqValidationFailedException>(async () => await customerService.Update(customer));

            // assert
            exception.Should().NotBeNull();
            exception.Messages.Should().NotBeNull();
            exception.Messages.Count.Should().Be(1);
        }

        [Xunit.Theory]
        [InlineData(1)]
        public async Task delete_customer_should_return_success(int id)
        {
            // arrange
            var customerService = new CustomerService(mockCustomerRepository.Object);

            mockCustomerRepository.Setup(c => c.Delete(id))
                .Verifiable();

            // act
            await customerService.Delete(id);

            // assert
            Xunit.Assert.True(true);
        }

        [Xunit.Theory]
        [InlineData(0)]
        [InlineData(-1)]

        public async Task delete_customer_should_throw_exception_if_invalid_request(int id)
        {
            // arrange
            var customerService = new CustomerService(mockCustomerRepository.Object);

            // act
            var exception = await Xunit.Assert.ThrowsAsync<RqValidationFailedException>(async () => await customerService.Delete(id));

            // assert
            exception.Should().NotBeNull();
            exception.Messages.Should().NotBeNull();
            exception.Messages.Count.Should().Be(1);
            exception.Messages[0].Message.Should().Be(Constants.CustomerIdRequiredMessage);
        }
    }
}
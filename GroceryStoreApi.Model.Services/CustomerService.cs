using GroceryStore.Data;
using GroceryStore.Data.Access.Interfaces;
using GroceryStore.Domain;
using GroceryStore.Services.Interfaces;
using GroceryStoreApi.Infrastructure;
using GroceryStoreApi.Infrastructure.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryStore.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public async Task<List<Customer>> GetAll()
        {
            var customers = await _customerRepository.GetAll();

            return customers.Select(c => new Customer
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
        }

        public async Task<Customer> GetById(int id)
        {
            var validations = new List<ValidationResult>();
            if( id <= 0)
            {
                validations.Add(new ValidationResult(Constants.CustomerIdRequiredMessage));
            }
            
            if(validations.Any())
            {
                throw new RqValidationFailedException(validations);
            }

            var customer = await _customerRepository.GetById(id);

            return customer == null ? null :new Customer
            {
                Id = customer.Id,
                Name = customer.Name
            };
        }

        public async Task<Customer> Add(Customer customer)
        {
            var validations = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                validations.Add(new ValidationResult(Constants.CustomerNameRequiredMessage));
            }

            if (validations.Any())
            {
                throw new RqValidationFailedException(validations);
            }

            var dbCustomer = await _customerRepository.Add(new CustomerEntity
            {
                Name = customer.Name
            });

            return new Customer
            {
                Id = dbCustomer.Id,
                Name = dbCustomer.Name
            };
        }

        public async Task Update(Customer customer)
        {
            var validations = new List<ValidationResult>();
            if (customer.Id <= 0)
            {
                validations.Add(new ValidationResult(Constants.CustomerIdRequiredMessage));
            }
            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                validations.Add(new ValidationResult(Constants.CustomerNameRequiredMessage));
            }

            if (validations.Any())
            {
                throw new RqValidationFailedException(validations);
            }

            await _customerRepository.Update(new CustomerEntity
            {
                Id = customer.Id,
                Name = customer.Name
            });
        }

        public async Task Delete(int id)
        {
            var validations = new List<ValidationResult>();
            if (id <= 0)
            {
                validations.Add(new ValidationResult(Constants.CustomerIdRequiredMessage));
            }
            if (validations.Any())
            {
                throw new RqValidationFailedException(validations);
            }

            await _customerRepository.Delete(id);
        }
    }
}

using GroceryStore.Data;
using GroceryStore.Data.Access.Interfaces;
using GroceryStore.Domain;
using GroceryStore.Services.Interfaces;
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
        public async Task<IEnumerable<Customer>> GetAll()
        {
            var customers = await _customerRepository.GetAll();

            return customers.Select(c => new Customer
            {
                Id = c.Id,
                Name = c.Name
            });
        }

        public async Task<Customer> GetById(int id)
        {
            var customer = await _customerRepository.GetById(id);

            return customer == null ? null :new Customer
            {
                Id = customer.Id,
                Name = customer.Name
            };
        }

        public async Task<Customer> Add(Customer customer)
        {
            var dbCustomer = await _customerRepository.Add(new CustomerEntity
            {
                Id = customer.Id,
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
            await _customerRepository.Update(new CustomerEntity
            {
                Id = customer.Id,
                Name = customer.Name
            });
        }

        public async Task Delete(int id)
        {
           await _customerRepository.Delete(id);
        }
    }
}

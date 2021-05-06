using GroceryStore.Data.Access.Interfaces;
using GroceryStoreApi.Data.Access.EFCore;
using GroceryStoreApi.Infrastructure;
using GroceryStoreApi.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryStore.Data.Access.Repo
{
    public class CustomerSqlRepository : ICustomerRepository
    {
        private readonly StoreDbContext _context;
        public CustomerSqlRepository()
        {
            _context = new StoreDbContext();
        }
        public async Task<List<CustomerEntity>> GetAll()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<CustomerEntity> GetById(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CustomerEntity> Add(CustomerEntity customerEntity)
        {
            bool exists = await _context.Customers.AnyAsync(c => c.Name == customerEntity.Name);
            if (exists)
            {
                throw new DuplicateCustomerException(Constants.CustomerAlreadyExists);
            }

            _context.Customers.Add(customerEntity);
            _context.SaveChanges();

            return await _context.Customers.FirstOrDefaultAsync(c => c.Name == customerEntity.Name);
        }

        public async Task Update(CustomerEntity customerEntity)
        {
            bool exists = _context.Customers.Any(c => c.Id == customerEntity.Id);
            if (!exists)
            {
                throw new CustomerNotFoundException(Constants.CustomerNotFoundMessage);
            }

            var customer = await _context.Customers.FirstAsync(c => c.Id == customerEntity.Id);
            customer.Name = customerEntity.Name;
            _context.SaveChanges();
        }

        public async Task Delete(int id)
        {
            bool exists = _context.Customers.Any(c => c.Id == id);
            if (!exists)
            {
                throw new CustomerNotFoundException(Constants.CustomerNotFoundMessage);
            }

            var customer = await _context.Customers.FirstAsync(c => c.Id == id);
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }
    }
}

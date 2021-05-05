using GroceryStore.Data.Access.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryStore.Data.Access.Repo
{
    public class CustomerSqlRepository : ICustomerRepository
    {
        public async Task<List<CustomerEntity>> GetAll()
        {
            return await Task.FromResult(new List<CustomerEntity> { });
        }

        public async Task<CustomerEntity> GetById(int id)
        {
            return await Task.FromResult(new CustomerEntity { Id = id, Name = "test" });
        }

        public async Task<CustomerEntity> Add(CustomerEntity customerEntity)
        {
            return await Task.FromResult(new CustomerEntity { Id = 2, Name = "test2" });
        }

        public async Task Update(CustomerEntity customerEntity)
        {
            await Task.FromResult(Task.CompletedTask);
        }

        public async Task Delete(int id)
        {
            await Task.FromResult(Task.CompletedTask);
        }
    }
}

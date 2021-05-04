using GroceryStore.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryStore.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAll();
        Task<Customer> GetById(int id);
        Task<Customer> Add(Customer customer);
        Task Update(Customer customer);
        Task Delete(int id);
    }
}

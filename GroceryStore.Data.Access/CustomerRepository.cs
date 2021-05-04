using GroceryStore.Data.Access.Interfaces;
using GroceryStoreApi.Infrastructure.Exceptions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryStore.Data
{
    public class CustomerRepository : ICustomerRepository
    {
        private const string dbFile = "database.json";
        public async Task<IEnumerable<CustomerEntity>> GetAll()
        {
            var data = ReadDataFromFile();

            return await Task.FromResult(data.Customers);
        }

        public async Task<CustomerEntity> GetById(int id)
        {
            var customers = await GetAll();

            var customer = customers.FirstOrDefault(c => c.Id == id);

            return await Task.FromResult(customer);
        }

        public async Task<CustomerEntity> Add(CustomerEntity customerEntity)
        {
            var data = ReadDataFromFile();
            if (data != null && data.Customers != null && data.Customers.Any())
            {
                // check for duplicate customer
                if (data.Customers.Exists(c => c.Name.Equals(customerEntity.Name, System.StringComparison.OrdinalIgnoreCase)))
                {
                    throw new DuplicateCustomerException();
                }
                var maxId = data.Customers.Max(c => c.Id);
                var nextId = maxId + 1;
                customerEntity.Id = nextId;
                data.Customers.Add(customerEntity);
                WriteDataToFile(data);
            }
            else
            {
                var customers = new ListCustomers();
                customerEntity.Id = 1;
                customers.Customers.Add(customerEntity);
                WriteDataToFile(customers);
            }

            return await Task.FromResult(customerEntity);
        }

        public async Task Update(CustomerEntity customerEntity)
        {
            var data = ReadDataFromFile();
            if (data != null && data.Customers != null && data.Customers.Any())
            {
                if (data.Customers.Exists(c => c.Id == customerEntity.Id))
                {
                    data.Customers.Where(c => c.Id == customerEntity.Id).First().Name = customerEntity.Name;

                    WriteDataToFile(data);
                }
                else
                {
                    throw new CustomerNotFoundException();
                }
            }

            await Task.FromResult(Task.CompletedTask);
        }

        public async Task Delete(int id)
        {
            var data = ReadDataFromFile();
            if (data != null && data.Customers != null && data.Customers.Any())
            {
                if (data.Customers.Exists(c => c.Id == id))
                {
                    var itemToRemove = data.Customers.First(c => c.Id == id);
                    data.Customers.Remove(itemToRemove);

                    WriteDataToFile(data);
                }
                else
                {
                    throw new CustomerNotFoundException();
                }
            }

            await Task.FromResult(Task.CompletedTask);
        }

        private ListCustomers ReadDataFromFile()
        {
            var json = File.ReadAllText(dbFile);
            var data = JsonConvert.DeserializeObject<ListCustomers>(json);

            return data;
        }

        private void WriteDataToFile(ListCustomers data)
        {
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(dbFile, json);
        }
    }
}

using GroceryStore.Domain;
using GroceryStore.Services.Interfaces;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using GroceryStoreApi.Infrastructure.Exceptions;

namespace GroceryStoreAPI.Controllers
{
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [Route("customers")]
        public async Task<IEnumerable<Customer>> Get()
        {
            return await _customerService.GetAll();
        }

        [HttpGet]
        [Route("/customers/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result =  await _customerService.GetById(id);
            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("/customers")]
        public async Task<ActionResult> Create([FromBody] Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                return BadRequest("invalid request. Customer name is required.");
            }

            try
            {
                var result = await _customerService.Add(customer);

                return Ok(result);
            }
            catch (DuplicateCustomerException)
            {
                return BadRequest("This customer already exists.");
            }
        }

        [HttpPut]
        [Route("/customers")]
        public async Task<ActionResult> Update([FromBody] Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.Name) || customer.Id <= 0)
            {
                return BadRequest("invalid request. Customer name and id is required.");
            }

            try
            {
                await _customerService.Update(customer);

                return Ok(HttpStatusCode.NoContent);
            }
            catch (CustomerNotFoundException)
            {
                return NotFound("Customer not found.");
            }
        }

        [HttpDelete]
        [Route("/customers/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if(id <= 0)
            {
                return BadRequest("Customer id is required");
            }

            try
            {
                await _customerService.Delete(id);

                return Ok(HttpStatusCode.NoContent);
            }
            catch (CustomerNotFoundException)
            {
                return NotFound("Customer not found.");
            }
        }
    }
}

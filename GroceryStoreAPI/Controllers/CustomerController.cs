using GroceryStore.Domain;
using GroceryStore.Services.Interfaces;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using GroceryStoreApi.Infrastructure.Exceptions;
using GroceryStoreApi.Infrastructure;
using System;

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
        public async Task<IActionResult> Get()
        {
            var result = await _customerService.GetAll();

            return Ok(result);
        }

        [HttpGet]
        [Route("/customers/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _customerService.GetById(id);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (RqValidationFailedException ex)
            {
                return BadRequest(ex.Messages);
            }
        }

        [HttpPost]
        [Route("/customers")]
        public async Task<ActionResult> Create([FromBody] Customer customer)
        {
            try
            {
                var result = await _customerService.Add(customer);

                return Ok(result);
            }
            catch (RqValidationFailedException ex)
            {
                return BadRequest(ex.Messages);
            }
            catch (DuplicateCustomerException ex)
            {
                return BadRequest(new ValidationResult(ex.Message));
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("/customers")]
        public async Task<ActionResult> Update([FromBody] Customer customer)
        {
            try
            {
                await _customerService.Update(customer);

                return Ok(HttpStatusCode.NoContent);
            }
            catch (RqValidationFailedException ex)
            {
                return BadRequest(ex.Messages);
            }
            catch (CustomerNotFoundException ex)
            {
                return NotFound(new ValidationResult(ex.Message));
            }
        }

        [HttpDelete]
        [Route("/customers/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _customerService.Delete(id);

                return Ok(HttpStatusCode.NoContent);
            }
            catch (RqValidationFailedException ex)
            {
                return BadRequest(ex.Messages);
            }
            catch (CustomerNotFoundException ex)
            {
                return NotFound(new ValidationResult(ex.Message));
            }
        }
    }
}

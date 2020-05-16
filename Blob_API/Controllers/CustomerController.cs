using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blob_API.Model;
using Microsoft.Extensions.Logging;

namespace Blob_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly BlobContext _context;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(BlobContext context, ILogger<CustomerController> logger)
        {
            _context = context;
            _logger = logger;
        }


        // GET api/Customers
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomersAsync()
        {
            return Ok(await _context.Customer.ToListAsync());
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        [ActionName(nameof(GetCustomerAsync))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Order>> GetCustomerAsync(uint id)
        {
            var customer = await _context.Customer.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // PUT: api/Customers
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutCustomerAsync([FromBody] IEnumerable<Customer> customersToUpdate)
        {
            // TODO: check/validate/sanitize values.



            foreach (var customerToUpdate in customersToUpdate)
            {
                if (customerToUpdate.Firstname == null) 
                {
                    return BadRequest("First name cannot be null.");
                }

                if (customerToUpdate.Lastname == null)
                {
                    return BadRequest("Last name cannot be null.");
                }

                if (!CustomerExists(customerToUpdate.Id))
                {
                    return NotFound("One or more objects did not exist in the Database, Id was not found.");
                }

                // Update customer and set state to Modified. 
                _context.Entry(customerToUpdate).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                _logger.LogError("DbUpdateConcurrencyException", e);
                return Problem("Could not save changes to Database", statusCode: 500, title: "Persistence Error");
            }
            catch (Exception exp)
            {
                _logger.LogError("Exception", exp);
                return Problem("Could not save changes to Database", statusCode: 500, title: "Persistence Error");
            }

            return NoContent();
        }

        // POST: api/Customers
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Customer>> PostOrderAsync(Customer customer)
        {
            // TODO: check/validate/sanitize values.

            if (customer.Firstname == null)
            {
                return BadRequest("First name cannot be null.");
            }

            if (customer.Lastname == null)
            {
                return BadRequest("Last name cannot be null.");
            }

            _context.Customer.Add(customer);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                _logger.LogError("DbUpdateConcurrencyException", e);
                return Problem("Could not save to Database", statusCode: 500, title: "Persistence Error");
            }
            catch (Exception exp)
            {
                _logger.LogError("Exception", exp);
                return Problem("Could not save to Database", statusCode: 500, title: "Persistence Error");
            }

            return CreatedAtAction(nameof(GetCustomerAsync), new { id = customer.Id }, customer);
        }


        private bool CustomerExists(uint id)
        {
            return _context.Customer.Any(e => e.Id == id);
        }
    }
}
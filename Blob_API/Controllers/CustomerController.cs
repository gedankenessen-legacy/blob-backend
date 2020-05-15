using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blob_API.Model;
using Microsoft.EntityFrameworkCore;
using System.CodeDom;

namespace Blob_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly BlobContext _context;

        public CustomerController(BlobContext context)
        {
            _context = context;
        }

        // GET api/order
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomersAsync()
        {
            // ? .Include(...) includes the elements of other tables to this 'query-object'.
            var customerList = await _context.Customer
                                    .Include(customer => customer.Address)
                                    .ToListAsync();

            return Ok(customerList);
        }

        // GET api/order/5
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Customer>> GetCustomersAsync(uint id)
        {
            // TODO: check/validate/sanitize values.
            if (id < 0)
            {
                return BadRequest();
            }
            var customer = await _context.Customer
                                    .Include(customer => customer.Address)
                                    .SingleAsync(customer => customer.Id == id);

            

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // POST api/order
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Order>>> CreateOrdersAsync([FromBody] Customer newCustomer)
        {
            // TODO: check/validate/sanitize values.

            var valueTask = await _context.Customer.AddAsync(newCustomer);

            await _context.SaveChangesAsync();

            var newCreatedCustomer = await _context.Customer
                                    .Include(customer => customer.Address)
                                    .SingleAsync(customer => customer.Id == valueTask.Entity.Id);

            return Created($"api/customer/{newCreatedCustomer.Id}", newCreatedCustomer);
        }
    }
}
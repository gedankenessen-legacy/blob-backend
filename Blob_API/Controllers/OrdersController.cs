using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blob_API.Model;

namespace Blob_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly BlobContext _context;

        public OrdersController(BlobContext context)
        {
            _context = context;
        }

        // GET: api/Order
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersAsync()
        {
            return await _context.Order.ToListAsync();
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Order>> GetOrderAsync(uint id)
        {
            var order = await _context.Order.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Order/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderAsync(uint id, [FromBody] Order order)
        {
            // TODO: check/validate/sanitize values.

            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Order
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<ActionResult<Order>> PostOrderAsync([FromBody] Order order)
        {
            // TODO: check/validate/sanitize values.

            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        private bool OrderExists(uint id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}

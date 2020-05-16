using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blob_API.Model;
using Microsoft.Extensions.Logging;
using Blob_API.Helpers;

namespace Blob_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly BlobContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(BlobContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Orders
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersAsync()
        {
            return Ok(await _context.Order.ToListAsync());
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        [ActionName(nameof(GetOrderAsync))]
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

            return Ok(order);
        }

        // PUT: api/Orders
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutOrderAsync([FromBody] IEnumerable<Order> ordersToUpdate)
        {
            // TODO: check/validate/sanitize values.

            foreach (var orderToUpdate in ordersToUpdate)
            {
                if (!OrderExists(orderToUpdate.Id))
                {
                    // Iterate throught every entry that was modified and reload the values from the Database
                    await DatabaseHelper.RevertValues(ordersToUpdate, _context);

                    return NotFound("One or more objects did not exist in the Database, Id was not found.");
                }

                // Update order and set state to Modified. 
                _context.Entry(orderToUpdate).State = EntityState.Modified;
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

        // POST: api/Orders
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Order>> PostOrderAsync(Order order)
        {
            // TODO: check/validate/sanitize values.

            _context.Order.Add(order);

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

            return CreatedAtAction(nameof(GetOrderAsync), new { id = order.Id }, order);
        }

        private bool OrderExists(uint id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}

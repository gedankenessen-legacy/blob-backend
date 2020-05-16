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
using AutoMapper;
using Blob_API.RessourceModels;

namespace Blob_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly BlobContext _context;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMapper _mapper;

        public OrdersController(BlobContext context, ILogger<OrdersController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/Orders
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<OrderRessource>>> GetOrdersAsync()
        {
            var orderList = await _context.Order.ToListAsync();

            IEnumerable<OrderRessource> orderRessourceList = _mapper.Map<IEnumerable<OrderRessource>>(orderList);

            return Ok(orderRessourceList);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        [ActionName(nameof(GetOrderAsync))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<OrderRessource>> GetOrderAsync(uint id)
        {
            var order = await _context.Order.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            var orderRessource = _mapper.Map<OrderRessource>(order);

            return Ok(orderRessource);
        }

        // PUT: api/Orders
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutOrderAsync([FromBody] IEnumerable<OrderRessource> orderRessourcesToUpdate)
        {
            var ordersToUpdate = _mapper.Map<IEnumerable<Order>>(orderRessourcesToUpdate);

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
                return Problem("Could not save changes to Database", statusCode: 500, title: "Error");
            }

            return NoContent();
        }

        // POST: api/Orders
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<OrderRessource>> PostOrderAsync(OrderRessource orderRessource)
        {
            var order = _mapper.Map<Order>(orderRessource);

            // TODO: check/validate/sanitize values.

            // TODO: S19.4: Create backup of products

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
                return Problem("Could not save to Database", statusCode: 500, title: "Error");
            }

            return CreatedAtAction(nameof(GetOrderAsync), new { id = order.Id }, order);
        }

        private bool OrderExists(uint id)
        {
            return _context.Order.Any(e => e.Id == id);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blob_API.Model;
using Microsoft.EntityFrameworkCore;

namespace Blob_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly BlobContext _context;

        public OrderController(BlobContext context)
        {
            _context = context;
        }

        // GET api/order
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrdersAsync()
        {
            // ? .Include(...) includes the elements of other tables to this 'query-object'.
            var orderList = await _context.Order
                                    .Include(order => order.Customer)
                                    .Include(order => order.OrderedCustomer)
                                    .Include(order => order.State)
                                    .Include(order => order.OrderedProductOrder)
                                        .ThenInclude(orderedProductOrder => orderedProductOrder.OrderedProduct)
                                    .ToListAsync();

            return Ok(orderList);
        }

        // GET api/order/5
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Order>> GetOrderAsync(int id)
        {
            var res = await _context.Order.FindAsync(id);

            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }

        // POST api/order
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Order>>> CreateOrdersAsync([FromBody] Order newOrder)
        {
            var res = await _context.Order.AddAsync(newOrder);

            await _context.SaveChangesAsync();

            return Created("", res);
        }
    }
}

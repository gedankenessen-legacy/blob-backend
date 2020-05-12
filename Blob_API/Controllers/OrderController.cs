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
            return Ok(await _context.Order.ToListAsync());
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

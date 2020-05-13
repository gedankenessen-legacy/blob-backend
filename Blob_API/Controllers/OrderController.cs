using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blob_API.Model;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Blob_API.RessourceModels;

namespace Blob_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly BlobContext _context;
        private readonly IMapper _mapper;

        public OrderController(BlobContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
        public async Task<ActionResult<Order>> GetOrderAsync(uint id)
        {
            // TODO: check/validate/sanitize values.

            var order = await _context.Order
                                    .Include(order => order.Customer)
                                    .Include(order => order.OrderedCustomer)
                                    .Include(order => order.State)
                                    .Include(order => order.OrderedProductOrder)
                                        .ThenInclude(orderedProductOrder => orderedProductOrder.OrderedProduct)
                                    .SingleAsync(order => order.Id == id); // Find() does not support a IIncludableQuerry in front of it.

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // POST api/order
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Order>>> CreateOrdersAsync([FromBody] Order newOrder)
        {
            // TODO: check/validate/sanitize values.

            var valueTask = await _context.Order.AddAsync(newOrder);

            await _context.SaveChangesAsync();

            var newCreatedOrder = await _context.Order
                                    .Include(order => order.Customer)
                                    .Include(order => order.OrderedCustomer)
                                    .Include(order => order.State)
                                    .Include(order => order.OrderedProductOrder)
                                    .ThenInclude(orderedProductOrder => orderedProductOrder.OrderedProduct)
                                    .SingleAsync(order => order.Id == valueTask.Entity.Id);

            return Created("", newCreatedOrder);
        }
    }
}

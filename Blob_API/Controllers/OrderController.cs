using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blob_API.Model;

namespace Blob_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private int test;
        private readonly BlobContext _context;

        public OrderController(BlobContext context)
        {
            _context = context;
        }

        // GET api/order
        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetAllOrders()
        {
            return _context.Order.ToList();
        }

        // GET api/order/5
        [HttpGet("{id}")]
        public ActionResult<Order> GetAllOrders(int id)
        {
            var res = _context.Order.Find(id);

            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }

        [HttpPost]
        public ActionResult<IEnumerable<Order>> CreateOrders([FromBody] Order newOrder)
        {
            var res = _context.Order.Select(order => order.Id == 5);

            _context.SaveChanges();

            return NoContent();
        }
    }
}

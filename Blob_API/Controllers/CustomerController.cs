using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blob_API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blob_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly BlobContext _context;

        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetAllCustomers()
        {
            return _context.Customer.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Order> GetCustomer(int id)
        {
            var res = _context.Customer.Find(id);

            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }


    }
}
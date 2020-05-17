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

        /// GET: api/Orders
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
        public async Task<IActionResult> PutOrderAsync([FromBody] IEnumerable<OrderRessource> orderRessources)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                // TODO: check/validate/sanitize values.

                // Check if the order is in the database.
                foreach (var orderRessource in orderRessources)
                {
                    if (!OrderExists(orderRessource.Id))
                    {
                        return NotFound("One or more objects did not exist in the Database, Id was not found.");
                    }
                }

                // Update entries
                foreach (var orderRessource in orderRessources)
                {
                    var entry = _context.Order.Find(orderRessource.Id);

                    var newCustomer = _mapper.Map<Customer>(orderRessource.Customer);
                    // Make sure the id still the same.
                    newCustomer.Id = entry.Customer.Id;
                    entry.Customer = newCustomer;

                    foreach (var orderedProduct in orderRessource.OrderedProducts)
                    {
                        // Check if product is already in OrderedProductOrder table.
                        OrderedProductOrder orderedProductOrder = _context.OrderedProductOrder.Find(orderedProduct.Id, orderRessource.Id);

                        // TODO: PUT creates the ressource if not found. Consider Idempotency.
                        if (orderedProductOrder == null)
                        {
                            return NotFound($"The ordered product with the ID={orderedProduct.Id} was not found.");
                        }

                        // Update values
                        var newOrderedProduct = _mapper.Map<OrderedProduct>(orderedProduct);

                        // Make sure the id still the same.
                        newOrderedProduct.Id = orderedProductOrder.OrderedProduct.Id;

                        orderedProductOrder.OrderedProduct = newOrderedProduct;
                        orderedProductOrder.Quantity = orderedProduct.Quantity;
                    }

                    // Update order and set state to Modified. 
                    _context.Entry(entry).State = EntityState.Modified;
                }

                await TryContextSaveAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
        }

        // POST: api/Orders
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<OrderRessource>> PostOrderAsync(OrderRessource orderRessource)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                // Check if the customer already exists.
                Customer customer = _context.Customer.Find(orderRessource.Customer.Id);
                if (customer == null)
                {
                    BadRequest("The customer does not exist.");
                }

                // Check if the address already exists.
                Address address = _context.Address.Find(orderRessource.Customer.Address.Id);
                if (address == null)
                {
                    BadRequest("The Address does not exist.");
                }

                Order newOrder = new Order
                {
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    Customer = customer
                };

                await _context.Order.AddAsync(newOrder);

                // Create "ghost/copy/backup"-Address
                OrderedAddress newOrderedAddress = new OrderedAddress()
                {
                    Street = address.Street,
                    Zip = address.Zip,
                    City = address.City
                };

                await _context.OrderedAddress.AddAsync(newOrderedAddress);
                //await TryContextSaveAsync();

                // Create "ghost/copy/backup"-Customer
                OrderedCustomer newOrderedCustomer = new OrderedCustomer()
                {
                    Firstname = customer.Firstname,
                    Lastname = customer.Lastname,
                    OrderedAddress = newOrderedAddress
                };
                await _context.OrderedCustomer.AddAsync(newOrderedCustomer);
                //await TryContextSaveAsync();

                newOrder.OrderedCustomer = newOrderedCustomer;
                newOrder.State = _context.State.First();


                // TODO: S19.4: Create backup of products
                OrderedProductRessource[] array = orderRessource.OrderedProducts.ToArray();

                for (int i = 0; i < array.Length; i++)
                {
                    OrderedProductRessource orderedProduct = array[i];
                    var orderedProductDTO = _mapper.Map<OrderedProduct>(orderedProduct);

                    // check if the product exists.
                    Product product = _context.Product.Find(orderedProduct.Id);
                    if (product == null)
                    {
                        return NotFound($"The ordered product with the ID={orderedProduct.Id} was not found.");
                    }

                    // Add "ghost/copy/backup"-Product if no entry exists.
                    uint orderedProductId = 0;
                    OrderedProduct ordProd = _context.OrderedProduct.Where(ordProd => ordProd == orderedProductDTO).First();
                    if ((orderedProductId = _context.OrderedProduct.Where(ordProd => ordProd == orderedProductDTO).First().Id) == 0)
                    {
                        // TODO: Check values, sanitize.
                        ordProd = new OrderedProduct()
                        {
                            Name = product.Name,
                            Price = product.Price,
                            Sku = product.Sku,
                            Product = product
                        };
                        await _context.OrderedProduct.AddAsync(ordProd);
                        //await TryContextSaveAsync();

                        orderedProductDTO = ordProd;
                    }


                    // Add orderedProduct to OrderedProductOrder-Table if no entry exists.
                    // BUG: id is always 0
                    var ordProdOrd = _context.OrderedProductOrder.Find(ordProd.Id, newOrder.Id);
                    if (ordProdOrd == null || ordProdOrd.Quantity != orderedProduct.Quantity)
                    {
                        OrderedProductOrder opo = new OrderedProductOrder()
                        {
                            OrderedProduct = ordProd,
                            Order = newOrder,
                            Quantity = orderedProduct.Quantity
                        };
                        await _context.OrderedProductOrder.AddAsync(opo);
                    }
                }

                await TryContextSaveAsync();

                await transaction.CommitAsync();
                return CreatedAtAction(nameof(GetOrderAsync), new { id = newOrder.Id }, newOrder);
            }
        }

        private bool OrderExists(uint id)
        {
            return _context.Order.Any(e => e.Id == id);
        }

        private async Task<ActionResult> TryContextSaveAsync()
        {
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

            return StatusCode(500);
        }
    }
}

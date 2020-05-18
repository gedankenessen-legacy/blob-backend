using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blob_API.Model;
using Microsoft.Extensions.Logging;
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

            await AddQuantityToOrder(orderRessourceList);

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

            await AddQuantityToOrder(Enumerable.Repeat(orderRessource, 1));

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
                // Update entries
                foreach (var orderRessource in orderRessources)
                {
                    if (!OrderExists(orderRessource.Id))
                    {
                        return NotFound("One or more objects did not exist in the Database, Id was not found.");
                    }

                    if (orderRessource.LocationId <= 0)
                    {
                        return BadRequest("No location id provided!");
                    }

                    var orderToUpdate = _context.Order.Find(orderRessource.Id);

                    foreach (var orderedProduct in orderRessource.OrderedProducts)
                    {
                        // Check if product is already in OrderedProductOrder table.
                        OrderedProductOrder ordProdOrd = _context.OrderedProductOrder.Find(orderedProduct.Id, orderRessource.Id);

                        // If not create it.
                        if (ordProdOrd == null)
                        {
                            ordProdOrd = new OrderedProductOrder()
                            {
                                OrderedProduct = _context.OrderedProduct.Find(orderedProduct.Id),
                                Order = orderToUpdate,
                                Quantity = orderedProduct.Quantity,
                            };
                            await _context.OrderedProductOrder.AddAsync(ordProdOrd);
                        }
                        else if (ordProdOrd.Quantity != orderedProduct.Quantity)
                        {
                            ordProdOrd.Quantity = orderedProduct.Quantity;
                        }

                    }
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
                    return BadRequest("The customer does not exist.");
                }

                // Check if the address already exists.
                Address address = _context.Address.Find(orderRessource.Customer.Address.Id);
                if (address == null)
                {
                    return BadRequest("The address does not exist.");
                }

                // Create "ghost/copy/backup"-Address
                // TODO: var newOrderedAddress =_mapper.Map<OrderedAddress>(address);
                OrderedAddress newOrderedAddress = new OrderedAddress()
                {
                    Street = address.Street,
                    Zip = address.Zip,
                    City = address.City
                };

                await _context.OrderedAddress.AddAsync(newOrderedAddress);

                // Create "ghost/copy/backup"-Customer
                OrderedCustomer newOrderedCustomer = new OrderedCustomer()
                {
                    Firstname = customer.Firstname,
                    Lastname = customer.Lastname,
                    OrderedAddress = newOrderedAddress
                };
                await _context.OrderedCustomer.AddAsync(newOrderedCustomer);

                Order newOrder = new Order
                {
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    Customer = customer,
                    State = _context.State.First(),
                    OrderedCustomer = newOrderedCustomer
                };

                await _context.Order.AddAsync(newOrder);

                // TODO: S19.4: Create backup of products
                OrderedProductRessource[] array = orderRessource.OrderedProducts.ToArray();

                for (int i = 0; i < array.Length; i++)
                {
                    OrderedProductRessource orderedProductRessource = array[i];
                    OrderedProduct orderedProduct = _mapper.Map<OrderedProduct>(orderedProductRessource);

                    // check if the product exists.
                    Product product = _context.Product.Find(orderedProductRessource.Id);
                    if (product == null)
                    {
                        return NotFound($"The ordered product with the ID={orderedProductRessource.Id} was not found.");
                    }

                    // Add "ghost/copy/backup"-Product if no entry exists.
                    uint orderedProductId = 0;
                    OrderedProduct ordProd = _context.OrderedProduct.Where(ordProd => ordProd == orderedProduct).First();
                    if ((orderedProductId = _context.OrderedProduct.Where(ordProd => ordProd == orderedProduct).First().Id) == 0)
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

                        orderedProduct = ordProd;
                    }


                    // Add orderedProduct to OrderedProductOrder-Table if no entry exists.
                    // BUG: id is always 0
                    var ordProdOrd = _context.OrderedProductOrder.Find(ordProd.Id, newOrder.Id);
                    if (ordProdOrd == null || ordProdOrd.Quantity != orderedProductRessource.Quantity)
                    {
                        OrderedProductOrder opo = new OrderedProductOrder()
                        {
                            OrderedProduct = ordProd,
                            Order = newOrder,
                            Quantity = orderedProductRessource.Quantity
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

        /// <summary>
        /// Iterates through all ordered product and sums up the quantity for each.
        /// </summary>
        /// <param name="orderRessourceList">A list of Orders</param>
        /// <returns>Awaitable Task</returns>
        private async Task AddQuantityToOrder(IEnumerable<OrderRessource> orderRessourceList)
        {
            // get orderedProduct quantity
            foreach (var orderRessource in orderRessourceList)
            {
                foreach (var orderedProduct in orderRessource.OrderedProducts)
                {
                    var ordProdOrd = await _context.OrderedProductOrder.FindAsync(orderedProduct.Id, orderRessource.Id);
                    if (ordProdOrd != null)
                        orderedProduct.Quantity = ordProdOrd.Quantity;
                }
            }
        }

        /// <summary>
        /// Reduces the stock of an item by the given quantity.
        /// </summary>
        /// <param name="quantity">The amount to reduce the stock.</param>
        /// <param name="locationId">The id of the location.</param>
        /// <param name="productId">The id of the product.</param>
        /// <returns>true if successful, and false with an error message if not. </returns>
        private async Task<(bool, string)> ReduceStockAsync(uint quantity, uint locationId, uint productId)
        {
            var locationProduct = await _context.LocationProduct.FindAsync(locationId, productId);

            if (locationProduct.Quantity >= quantity)
            {
                locationProduct.Quantity -= quantity;
                return (true, "");
            }
            else
                return (false, "Not enough items in stock.");
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

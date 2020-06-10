using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blob_API.Model;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation;
using AutoMapper;
using Blob_API.RessourceModels;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.CodeAnalysis;

namespace Blob_API.Controllers
{
    [Route("api/[controller]")]
    //[Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly BlobContext _context;
        private readonly ILogger<CustomerController> _logger;
        private readonly IMapper _mapper;

        public CustomerController(BlobContext context, ILogger<CustomerController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }


        // GET api/Customers
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomersAsync()
        {
            var customerList = await _context.Customer.ToListAsync();

            IEnumerable<CustomerRessource> customerRessourceList = _mapper.Map<IEnumerable<CustomerRessource>>(customerList);

            return Ok(customerRessourceList);

        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        [ActionName(nameof(GetCustomerAsync))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Order>> GetCustomerAsync(uint id)
        {
            var customer = await _context.Customer.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }


            var customerRessource = _mapper.Map<CustomerRessource>(customer);

            return Ok(customerRessource);
        }

        // PUT: api/Customers
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutCustomerAsync([FromBody] IEnumerable<CustomerRessource> customerRessources)
        {
            // TODO: check/validate/sanitize values.

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {

                foreach (var customerRessource in customerRessources)
                {
                    if (customerRessource.Firstname == null)
                    {
                        return BadRequest("First name cannot be null.");
                    }

                    if (customerRessource.Lastname == null)
                    {
                        return BadRequest("Last name cannot be null.");
                    }

                    if (!CustomerExists(customerRessource.Id))
                    {
                        return NotFound("One or more objects did not exist in the Database, Id was not found.");
                    }
                    Address address = _context.Address.Find(customerRessource.Address.Id);

                    //Neue Adresse kreieren, falls sie noch nicht existiert
                    if (address == null)
                    {
                        Address newAddress = customerRessource.Address;

                        await _context.Address.AddAsync(newAddress);
                    }
                    else
                    {
                        if (address.Street != customerRessource.Address.Street)
                        {
                            address.Street = customerRessource.Address.Street;
                        }
                        if (address.Location != customerRessource.Address.Location)
                        {
                            address.Location = customerRessource.Address.Location;
                        }
                        if (address.Zip != customerRessource.Address.Zip)
                        {
                            address.Zip = customerRessource.Address.Zip;
                        }
                        if (address.City != customerRessource.Address.City)
                        {
                            address.City = customerRessource.Address.City;
                        }
                    }



                    var customerToUpdate = _context.Customer.Find(customerRessource.Id);

                    if (customerToUpdate.Firstname != customerRessource.Firstname)
                    {
                        customerToUpdate.Firstname = customerRessource.Firstname;
                    }

                    if (customerToUpdate.Lastname != customerRessource.Lastname)
                    {
                        customerToUpdate.Firstname = customerRessource.Firstname;
                    }




                    // Update customer and set state to Modified. 
                    //_context.Entry(customerRessource).State = EntityState.Modified;
                }

                await TryContextSaveAsync();
                await transaction.CommitAsync();

                return NoContent();
            }

        }

        // POST: api/Customers
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Customer>> PostOrderAsync(CustomerRessource customerRessource)
        {
            // TODO: check/validate/sanitize values.
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                if (customerRessource.Firstname == null)
                {
                    return BadRequest("First name cannot be null.");
                }

                if (customerRessource.Lastname == null)
                {
                    return BadRequest("Last name cannot be null.");
                }

                Address address = _context.Address.Find(customerRessource.Address.Id);
                if (address == null)
                {
                    Address newAddress = customerRessource.Address;

                    await _context.Address.AddAsync(newAddress);
                }

                Customer newCustomer = new Customer
                {
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    Firstname = customerRessource.Firstname,
                    Lastname = customerRessource.Lastname,
                    Address = customerRessource.Address,
                    AddressId = customerRessource.Address.Id,

                };

                await _context.Customer.AddAsync(newCustomer);

                await TryContextSaveAsync();

                await transaction.CommitAsync();


                return CreatedAtAction(nameof(GetCustomerAsync), new { id = newCustomer.Id }, newCustomer);
            }
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Order>> DeleteCustomerAsync(uint id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var customer = await _context.Customer.FindAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }
                _context.Customer.Remove(customer);

                await TryContextSaveAsync();

                await transaction.CommitAsync();

                return NoContent();
            } 
           
        }



        private bool CustomerExists(uint id)
        {
            return _context.Customer.Any(e => e.Id == id);
        }

        private bool AdressExists(Address adress)
        {
            return _context.Address.Any(e => e.Id == adress.Id);
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
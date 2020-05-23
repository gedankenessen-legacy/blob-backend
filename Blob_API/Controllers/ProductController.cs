using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blob_API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenIddict.Validation;

namespace Blob_API.Controllers
{
    [Route("api/[controller]")]
    //[Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly BlobContext _context;
        private readonly ILogger<ProductController> _logger;

        public ProductController(BlobContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            return Ok(await _context.Product.ToListAsync());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Product>> GetProduct(uint id)
        {
            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound("One or more objects did not exist in the Database, Id was not found.");
            }

            return Ok(product);
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutProductAsync([FromBody] IEnumerable<Product> productsToUpdate)
        {
            // TODO: check/validate/sanitize values.

            foreach (var productToUpdate in productsToUpdate)
            {
                if (productToUpdate.Name == null)
                {
                    return Problem("The Products Name can not be null", statusCode: 404, title: "User Error");
                }

                if (productToUpdate.Price < 0)
                {
                    return Problem("The Products Price can not be under 0", statusCode: 404, title: "User Error");
                }

                if (!ProductExists(productToUpdate.Id))
                {
                    return NotFound("One or more objects did not exist in the Database, Id was not found.");
                }

                // Update product and set state to Modified. 
                _context.Entry(productToUpdate).State = EntityState.Modified;
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

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Product>> PostProductAsync(Product product)
        {
            
            if (product.Name == null)
            {
                return Problem("The Products Name can not be null", statusCode: 404, title: "User Error");
            }

            if (product.Price < 0)
            {
                return Problem("The Products Price can not be under 0", statusCode: 404, title: "User Error");
            }

            _context.Product.Add(product);

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

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProduct(uint id)
        {

            if (!ProductExists(id))
            {
                return NotFound("One or more objects did not exist in the Database, Id was not found.");
            }

            _context.Product.Remove(_context.Product.Find(id));

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

            return NoContent();
        }

        private bool ProductExists(uint id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
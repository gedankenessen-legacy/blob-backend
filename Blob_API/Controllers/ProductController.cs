using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blob_API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blob_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly BlobContext _context;

        public ProductController(BlobContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            var productList = await _context.Product
                .Include(product => product.CategoryProduct)
                    .ThenInclude(CategoryProduct => CategoryProduct.Category)
                .Include(product => product.LocationProduct)
                    .ThenInclude(LocationProduct => LocationProduct.Location)
                .Include(product => product.ProductProperty)
                    .ThenInclude(ProductProperty => ProductProperty.Property)
                .ToListAsync();

            return Ok(productList);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            var product = await _context.Product
                .Include(product => product.CategoryProduct)
                    .ThenInclude(CategoryProduct => CategoryProduct.Category)
                .Include(product => product.LocationProduct)
                    .ThenInclude(LocationProduct => LocationProduct.Location)
                .Include(product => product.ProductProperty)
                    .ThenInclude(ProductProperty => ProductProperty.Property)
                .SingleAsync(product => product.Id == id);


            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPut]
        
        public async Task<ActionResult<IEnumerable<Product>>> UpdateProducts( IEnumerable<Product> products)
        {
            //TODO Implementierung der Methode
            var test = _context.Product;
            return null;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Product>>> CreateProduct([FromBody] Product newProduct)
        {
            if (newProduct.Sku == null)
            {
                return BadRequest();
            }

            if (newProduct.CreatedAt == null)
            {
                return BadRequest();
            }

            var valueTask = await _context.Product.AddAsync(newProduct);

            await _context.SaveChangesAsync();

            var newCreatedProduct = await _context.Product
                .Include(product => product.CategoryProduct)
                    .ThenInclude(CategoryProduct => CategoryProduct.Category)
                .Include(product => product.LocationProduct)
                    .ThenInclude(LocationProduct => LocationProduct.Location)
                .Include(product => product.ProductProperty)
                    .ThenInclude(ProductProperty => ProductProperty.Property)
                .SingleAsync(product => product.Id == valueTask.Entity.Id);

            return Created($"api/product/{newCreatedProduct.Id}", newCreatedProduct);
        }

        [HttpDelete("{id}")]
        public void DeleteProduct(int id)
        {
            var res = _context.Product.Find(id);

            if (res != null)
            {
                _context.Product.Remove(res);
                _context.SaveChanges();
            }
            else
            {
                //TODO Eine Fehler Meldung Hinzufügen  
            }
        }

    }
}
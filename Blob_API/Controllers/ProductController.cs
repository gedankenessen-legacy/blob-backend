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
    public class ProductController : ControllerBase
    {
        private readonly BlobContext _context;

        public ProductController(BlobContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAllProducts()
        {
            return _context.Product.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            var res = _context.Product.Find(id);

            if (res == null)
            {
                return NotFound();
            }

            return Ok(res);
        }

        [HttpPut]
        public ActionResult<IEnumerable<Product>> UpdateProducts(IEnumerable<Product> products)
        {
            //TODO Implementierung der Methode
            return null;
        }

        [HttpPost]
        public ActionResult<Product> CreateProduct(Product product)
        {
            //TODO Implementierung der Methode
            return null;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blob_API.Model;
using Microsoft.AspNetCore.Authorization;
using Blob_API.RessourceModels;
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
        private readonly IMapper _mapper;

        public ProductController(BlobContext context, ILogger<ProductController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<ProductRessource>>> GetAllProducts()
        {
            var productList = await _context.Product.ToListAsync();

            IEnumerable<ProductRessource> productRessourcesList = _mapper.Map<IEnumerable<ProductRessource>>(productList);
            
            return Ok(productRessourcesList);
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetProductAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProductRessource>> GetProductAsync(uint id)
        {
            var product = await _context.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound("One or more objects did not exist in the Database, Id was not found.");
            }

            var productRessource = _mapper.Map<ProductRessource>(product);

            return Ok(productRessource);
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutProductAsync([FromBody] IEnumerable<ProductRessource> productRessources)
        {
            // TODO: check/validate/sanitize values.
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                foreach (var productRessource in productRessources)
                {
                    if (!ProductExists(productRessource.Id))
                    {
                        return NotFound("One or more objects did not exist in the Database, Id was not found.");
                    }

                    var productToUpdate = _context.Product.Find(productRessource.Id);

                    if (productRessource.Name != null)
                    {
                        productToUpdate.Name = productRessource.Name;
                    }

                    if (productRessource.Price < 0)
                    {
                        return Problem("The Products Price can not be under 0", statusCode: 404, title: "User Error");
                    }
                    else
                    {
                        productToUpdate.Price = productRessource.Price;
                    }

                    if (productRessource.ProductsAtLocations.Count == 0)
                    {
                        return BadRequest("Dem Produkt muss mindestens einen Standort zugewissen sein");
                    }

                    //TODO Update Stock
                    DeleteProductProperty(productToUpdate);
                    await TryContextSaveAsync();
                    if (productRessource.Properties != null)
                    {
                        await AddPropertyToProduct(productToUpdate, productRessource);
                    }

                    DeleteProductCategory(productToUpdate);
                    await TryContextSaveAsync();
                    if (productRessource.Categories != null)
                    {
                        await AddCategoryToProduct(productToUpdate, productRessource);
                    }

                    DeleteProductLocation(productToUpdate);
                    await TryContextSaveAsync();
                    if (productRessource.ProductsAtLocations != null)
                    {
                        await AddLocationToProduct(productToUpdate, productRessource);
                    }
                   
                   
                }

                await TryContextSaveAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProductRessource>> PostProductAsync(ProductRessource productRessource)
        {

            // TODO: check/validate/sanitize values.
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {

                if (productRessource.Name == null)
                {
                    return Problem("The Products Price can not be under 0", statusCode: 404, title: "User Error");
                }

                if (productRessource.Price < 0)
                {
                    return Problem("The Products Price can not be under 0", statusCode: 404, title: "User Error");
                }

                if (productRessource.ProductsAtLocations.Count == 0)
                {
                    return BadRequest("Dem Produkt muss mindestens einen Standort zugewissen sein");
                }

                var newProduct = new Product()
                {
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    Name = productRessource.Name,
                    Price = productRessource.Price,
                    Sku = productRessource.Sku
                };

                await _context.Product.AddAsync(newProduct);

                if (productRessource.Properties != null)
                {
                    await AddPropertyToProduct(newProduct, productRessource);
                }

                if (productRessource.Categories != null)
                {
                    await AddCategoryToProduct(newProduct, productRessource);
                }

                if (productRessource.ProductsAtLocations != null)
                {
                    await AddLocationToProduct(newProduct, productRessource);
                }

                await TryContextSaveAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetProductAsync), new {id = newProduct.Id}, newProduct);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProduct(uint id)
        {

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                if (!ProductExists(id))
                {
                    return NotFound("One or more objects did not exist in the Database, Id was not found.");
                }

                var productToDelete = _context.Product.Find(id);

                DeleteProductProperty(productToDelete);
                DeleteProductCategory(productToDelete);
                DeleteProductLocation(productToDelete);
                await TryContextSaveAsync();

                _context.Product.Remove(productToDelete);

                await TryContextSaveAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
        }

        private bool ProductExists(uint id)
        {
            return _context.Product.Any(e => e.Id == id);
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

        private async void AddToProductCategoryTable(Product product, Category category)
        {
            var found = _context.CategoryProduct.Find(category.Id, product.Id);
            if (found == null)
            {
                await _context.CategoryProduct.AddAsync(new CategoryProduct()
                {
                    Category = category,
                    Product = product
                });
            }
            // else already set.

            
        }

        private async void AddToProductPropertyTable(Product product, Property property)
        {
            var found = _context.ProductProperty.Find(product.Id, property.Id);

            if (found == null)
                await _context.ProductProperty.AddAsync(new ProductProperty()
                {
                    Product = product,
                    Property = property
                });
        }

        private void DeleteProductProperty(Product product)
        {
            if (product.ProductProperty != null)
            {
                foreach (var productProperty in product.ProductProperty)
                {
                    var PP = _context.ProductProperty.Find(product.Id, productProperty.PropertyId);

                   

                    if (PP != null)
                    {
                        _context.Entry(PP).State = EntityState.Detached;
                        _context.ProductProperty.Remove(PP);
                    }
                    
                } 
            }
        }

        private void DeleteProductCategory(Product product)
        {
            if (product.CategoryProduct != null)
            {
                foreach (var productCategory in product.CategoryProduct)
                {
                    var CP = _context.CategoryProduct.Find(productCategory.CategoryId, product.Id);

                    if (CP != null)
                    {
                        _context.Entry(CP).State = EntityState.Detached;
                        _context.CategoryProduct.Remove(CP);
                    }
                }
            }
        }

        private void DeleteProductLocation(Product product)
        {
            if (product.LocationProduct != null)
            {
                foreach (var locationProduct in product.LocationProduct)
                {
                    var LP = _context.LocationProduct.Find(locationProduct.LocationId, product.Id);


                    if (LP != null)
                    {
                        _context.Entry(LP).State = EntityState.Detached;
                        _context.LocationProduct.Remove(LP);
                    }
                }
            }
        }

        private async Task<ActionResult> AddPropertyToProduct(Product product, ProductRessource productRessource)
        {
            foreach (var productProperty in productRessource.Properties)
            {
                //Nach der Property suchen 
                var property = _context.Property.Find(productProperty.Id);

                //Wenn die Property nicht existiert: Erstelle Property und füge hinzu
                if (property == null)
                {
                    //Bei der Erstellung einer Property müssen Name und Value angegeben werden.
                    if (productProperty.Name == null)
                    {
                        return BadRequest();
                    }

                    if (productProperty.Value == null)
                    {
                        return BadRequest();
                    }

                    //Neue Property erstellen 
                    var newProperty = new Property()
                    {
                        Name = productProperty.Name,
                        Value = productProperty.Value
                    };

                    //Eintrag in die Tabellen Property und ProductProperty 
                    await _context.Property.AddAsync(newProperty);
                    AddToProductPropertyTable(product, newProperty);
                }
                //Die Property existiert
                else
                {
                    //Falls Property Geändert wird: Änderungen übernehmen 
                    if (productProperty.Name != null)
                    {
                        property.Name = productProperty.Name;
                    }

                    if (productProperty.Value != null)
                    {
                        property.Value = productProperty.Value;
                    }

                    AddToProductPropertyTable(product, property);

                }
            }

            return NoContent();
        }

        private async Task<ActionResult> AddCategoryToProduct(Product product, ProductRessource productRessource)
        {
            foreach (var productCategory in productRessource.Categories)
            {
                var category = _context.Category.Find(productCategory.Id);

                if (category == null)
                {

                    if (productCategory.Name == null)
                    {
                        return BadRequest();
                    }

                    var newCategory = new Category()
                    {
                        Name = productCategory.Name
                    };

                    await _context.Category.AddAsync(newCategory);
                    AddToProductCategoryTable(product, newCategory);
                }
                else
                {
                    if (productCategory.Name != null)
                    {
                        category.Name = productCategory.Name;
                    }

                    AddToProductCategoryTable(product, category);

                }
            }

            return NoContent();
        }

        private async Task<ActionResult> AddLocationToProduct(Product product, ProductRessource productRessource)
        {
            foreach (var productLocation in productRessource.ProductsAtLocations)
            {
                var location =
                    await _context.Location.FindAsync(productLocation.LocationId);

                if (location == null)
                {
                    return BadRequest("Die angegeben Location existiert nicht!");
                }

                if (productLocation.Quantity < 0)
                {
                    return BadRequest("The Anzahl can not be under 0 ");
                }

                await _context.LocationProduct.AddAsync(new LocationProduct()
                {
                    Location = location,
                    Product = product,
                    Quantity = productLocation.Quantity
                });
            }

            return NoContent();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blob_API.Model;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Blob_API.RessourceModels;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.CodeAnalysis;
using Location = Blob_API.Model.Location;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Blob_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly BlobContext _context;
        private readonly ILogger<LocationController> _logger;
        private readonly IMapper _mapper;

        public LocationController(BlobContext context, ILogger<LocationController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }


        // GET api/Locations

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<LocationRessource>>> GetAllProducts()
        {
            var locationList = await _context.Location.ToListAsync();

            IEnumerable<LocationRessource> locationRessourcesList = _mapper.Map<IEnumerable<LocationRessource>>(locationList);

            return Ok(locationRessourcesList);
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetLocationAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<LocationRessource>> GetLocationAsync(uint id)
        {
            var location = await _context.Location.FindAsync(id);

            if (location == null)
            {
                return NotFound("One or more objects did not exist in the Database, Id was not found.");
            }

            var locationRessource = _mapper.Map<LocationRessource>(location);

            return Ok(locationRessource);
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutProductAsync([FromBody] IEnumerable<LocationRessource> locationRessources)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {

                foreach (var locationRessource in locationRessources)
                {
                    if (!LocationExists(locationRessource.Id))
                    {
                        return NotFound("One or more objects did not exist in the Database, Id was not found.");
                    }

                    var locationToUpdate = _context.Location.Find(locationRessource.Id);

                    if (locationToUpdate.Name != null)
                    {
                        locationToUpdate.Name = locationRessource.Name;
                    }

                    var address = _context.Address.Find(locationRessource.AddressId);

                    //Falls Adresse nicht existiert: Erstelle Adresse 
                    if (address == null)
                    {
                        var addressRessorce = locationRessource.Address;

                        if (addressRessorce == null)
                        {
                            return BadRequest("Die Adresse existiert nicht, bitte erstellen Sie eine Adresse mit validen Daten");
                        }

                        if (addressRessorce.Street == null)
                        {
                            return BadRequest("Eine Adresse muss in einer Straße sein");
                        }

                        if (addressRessorce.City == null)
                        {
                            return BadRequest("Eine Adresse muss in einer Stadt sein");
                        }

                        if (addressRessorce.Zip == null)
                        {
                            return BadRequest("Eine Adresse muss eine PLZ haben");
                        }

                        var newAddress = new Address()
                        {
                            Street = addressRessorce.Street,
                            City = addressRessorce.City,
                            Zip = addressRessorce.Zip
                        };

                        await _context.Address.AddAsync(newAddress);

                        locationToUpdate.Address = newAddress;
                    }
                    else
                    {
                        var addressRessorce = locationRessource.Address;

                        if (addressRessorce.Street != null)
                        {
                            locationToUpdate.Address.Street = addressRessorce.Street;
                        }

                        if (addressRessorce.City != null)
                        {
                            locationToUpdate.Address.City = addressRessorce.City;
                        }

                        if (addressRessorce.Zip != null)
                        {
                            locationToUpdate.Address.Zip = addressRessorce.Zip;
                        }

                        locationToUpdate.AddressId = locationRessource.AddressId;
                    }

                    locationToUpdate.AddressId = locationRessource.AddressId;

                }

                await TryContextSaveAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<LocationRessource>> PostLocationAsync(LocationRessource locationRessource)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                if (locationRessource.Name == null)
                {
                    return Problem("Die Location muss ein Name haben!", statusCode: 404, title: "User Error");
                }

               

                var newLocation = new Location()
                {
                    Name = locationRessource.Name
                };

                await _context.Location.AddAsync(newLocation);

                var address = _context.Address.Find(locationRessource.AddressId);

                //Falls Adresse nicht existiert: Erstelle Adresse 
                if (address == null)
                {
                    var addressRessorce = locationRessource.Address;

                    if (addressRessorce == null)
                    {
                        return BadRequest("Die Adresse existiert nicht, bitte erstellen Sie eine Adresse mit validen Daten");
                    }

                    if (addressRessorce.Street == null)
                    {
                        return BadRequest("Eine Adresse muss in einer Straße sein");
                    }

                    if (addressRessorce.City == null)
                    {
                        return BadRequest("Eine Adresse muss in einer Stadt sein");
                    }

                    if (addressRessorce.Zip == null)
                    {
                        return BadRequest("Eine Adresse muss eine PLZ haben");
                    }

                    var newAddress = new Address()
                    {
                        Street = addressRessorce.Street,
                        City = addressRessorce.City,
                        Zip = addressRessorce.Zip
                    };

                    await _context.Address.AddAsync(newAddress);

                    locationRessource.Address = newAddress;
                }
                else
                {
                    var addressRessorce = locationRessource.Address;

                    if (addressRessorce.Street != null)
                    {
                        newLocation.Address.Street = addressRessorce.Street;
                    }

                    if (addressRessorce.City != null)
                    {
                        newLocation.Address.City = addressRessorce.City;
                    }

                    if (addressRessorce.Zip != null)
                    {
                        newLocation.Address.Zip = addressRessorce.Zip;
                    }

                    newLocation.AddressId = locationRessource.AddressId;
                }

                newLocation.AddressId = locationRessource.AddressId;
                await TryContextSaveAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetLocationAsync), new { id = newLocation.Id }, newLocation);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteLocation(uint id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                if (!LocationExists(id))
                {
                    return NotFound("One or more objects did not exist in the Database, Id was not found.");
                }

                var locationToDelete = _context.Location.Find(id);

                if (locationToDelete.LocationProduct.Count != 0)
                {
                    return BadRequest(
                        "Es darf kein Produkt mehr an diesem Standort vorhanden sein bevor es gelöscht werden kann");
                }

                _context.Location.Remove(locationToDelete);
                
                await TryContextSaveAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
        }

        private bool LocationExists(uint id)
        {
            return _context.Location.Any(e => e.Id == id);
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
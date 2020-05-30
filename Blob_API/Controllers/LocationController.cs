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
        public async Task<ActionResult<IEnumerable<Model.Location>>> GetLocationsAsync()
        {

            var locationList = await _context.Location.ToListAsync();
            return Ok(locationList);

        }

        // GET: api/Locations/5
        [HttpGet("{id}")]
        [ActionName(nameof(GetCustomerAsync))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Order>> GetCustomerAsync(uint id)
        {
            var location = await _context.Location.FindAsync(id);

            if (location == null)
            {
                return NotFound();
            }

            return Ok(location);
        }

        
}
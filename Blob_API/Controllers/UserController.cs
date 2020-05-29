using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blob_API.AuthModel;
using Blob_API.RessourceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blob_API.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public UserController(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: api/user
        [HttpGet]
        [ActionName(nameof(GetAllUsersAsync))]
        public async Task<ActionResult<IEnumerable<UserRessource>>> GetAllUsersAsync()
        {
            var allUsers = await _userManager.Users.ToListAsync();

            return Ok(_mapper.Map<IEnumerable<UserRessource>>(allUsers));
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        [ActionName(nameof(GetUser))]
        public ActionResult<UserRessource> GetUser(uint id)
        {
            var user = _userManager.Users.Where(u => u.Id == id).FirstOrDefault();

            if (user == null)
            {
                // ! TODO: security risk => unused userid´s could be exposed.
                NotFound($"User with the ID={id} not found.");
            }

            return Ok(_mapper.Map<UserRessource>(user));
        }

        // POST: api/user
        [HttpPost]
        public async Task<ActionResult<UserRessource>> CreateUserAsync([FromBody] UserRegisterForm userRegisterForm)
        {
            var newUser = new User()
            {
                FirstName = userRegisterForm.FirstName,
                LastName = userRegisterForm.LastName,
                UserName = await GenerateUserNameAsync(userRegisterForm.FirstName,userRegisterForm.LastName),
                CreatedAt = DateTime.Now.ToUniversalTime()
            };

            var res = await _userManager.CreateAsync(newUser, userRegisterForm.Password);

            if (!res.Succeeded)
            {
                return BadRequest("Unable to create user.");
            }

            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, _mapper.Map<UserRessource>(newUser));
        }

        /// <summary>
        /// Generates a unique Username, based of the Firstname and the Lastname. If the Firstname letters are not enough to build a unique name, use numbers at the end of the Username.
        /// </summary>
        /// <param name="firstName">Firstname</param>
        /// <param name="lastName">Lastname</param>
        /// <returns>Unique Username with Firstname+Lastname plus number suffix if needed.</returns>
        private async Task<string> GenerateUserNameAsync(string firstName, string lastName)
        {
            int index = 0;
            List<User> res;
            bool found = false;

            do
            {
                index++;
                res = await _userManager.Users.ToListAsync();
                if (index < firstName.Length)
                    found = res.Select(x => x.UserName == firstName.Substring(0, index) + lastName).Any(y => y == true); 
                else
                {
                    found = res.Select(x => x.UserName == firstName + lastName + index.ToString()).Any(y => y == true);
                }
            } while (found);

            return firstName.Substring(0, index) + lastName;
        }
    }

    public class UserRegisterForm
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
    }
}
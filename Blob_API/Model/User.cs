using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blob_API.Model
{
    public class User : IdentityUser<uint>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}

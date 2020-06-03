using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blob_API.AuthModel
{
    public class UserRole : IdentityRole<uint>
    {
        public UserRole() : base()
        {
        }

        public UserRole(string roleName) : base(roleName)
        {
        }
    }
}

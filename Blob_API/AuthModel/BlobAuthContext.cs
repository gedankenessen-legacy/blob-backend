using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blob_API.AuthModel
{
    public class BlobAuthContext : IdentityDbContext<User, UserRole, uint>
    {
        public BlobAuthContext(DbContextOptions<BlobAuthContext> options) : base(options)
        {
        }
    }
}

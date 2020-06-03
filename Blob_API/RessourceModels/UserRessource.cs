using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blob_API.RessourceModels
{
    public class UserRessource
    {
        public uint Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public string Email { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Blob_API.Model;

namespace Blob_API.RessourceModels
{
    public class LocationRessource
    {
        public LocationRessource()
        {
        }

        public uint Id { get; set; }
        public string Name { get; set; }
        
        public uint AddressId { get; set; }
       
        public virtual Address Address { get; set; }
       
        //public virtual ICollection<LocationProduct> LocationProduct { get; set; }
    }
}

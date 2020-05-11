using System;
using System.Collections.Generic;

namespace Blob_API.Model
{
    public partial class Location
    {
        public Location()
        {
            LocationProduct = new HashSet<LocationProduct>();
        }

        public uint Id { get; set; }
        public string Name { get; set; }
        public uint AddressId { get; set; }

        public virtual Address Address { get; set; }
        public virtual ICollection<LocationProduct> LocationProduct { get; set; }
    }
}

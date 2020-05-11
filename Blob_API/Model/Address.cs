using System;
using System.Collections.Generic;

namespace Blob_API.Model
{
    public partial class Address
    {
        public Address()
        {
            Customer = new HashSet<Customer>();
            Location = new HashSet<Location>();
        }

        public uint Id { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }

        public virtual ICollection<Customer> Customer { get; set; }
        public virtual ICollection<Location> Location { get; set; }
    }
}

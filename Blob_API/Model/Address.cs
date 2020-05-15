using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Blob_API.Model
{
    [DataContract]
    public partial class Address
    {
        public Address()
        {
            Customer = new HashSet<Customer>();
            Location = new HashSet<Location>();
        }

        [DataMember]
        public uint Id { get; set; }
        [DataMember]
        public string Street { get; set; }
        [DataMember]
        public string Zip { get; set; }
        [DataMember]
        public string City { get; set; }

        public virtual ICollection<Customer> Customer { get; set; }
        public virtual ICollection<Location> Location { get; set; }
    }
}

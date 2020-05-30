using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Blob_API.Model
{
    [DataContract]
    public partial class Location
    {
        public Location()
        {
            LocationProduct = new HashSet<LocationProduct>();
        }

        [DataMember]
        public uint Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public uint AddressId { get; set; }

        public virtual Address Address { get; set; }
        [JsonIgnore]
        public virtual ICollection<LocationProduct> LocationProduct { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Blob_API.Model
{
    [DataContract]
    public partial class OrderedCustomer
    {
        public OrderedCustomer()
        {
            Order = new HashSet<Order>();
        }

        [DataMember]
        public uint Id { get; set; }
        [DataMember]
        public string Firstname { get; set; }
        [DataMember]
        public string Lastname { get; set; }
        [DataMember]
        public uint OrderedAddressId { get; set; }

        public virtual OrderedAddress OrderedAddress { get; set; }
        [JsonIgnore]
        public virtual ICollection<Order> Order { get; set; }
    }
}

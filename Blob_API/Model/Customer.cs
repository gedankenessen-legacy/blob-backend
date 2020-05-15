using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Blob_API.Model
{
    [DataContract]
    public partial class Customer
    {
        public Customer()
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
        public DateTime? CreatedAt { get; set; }
        [DataMember]
        public uint AddressId { get; set; }

        public virtual Address Address { get; set; }
        public virtual ICollection<Order> Order { get; set; }
    }
}

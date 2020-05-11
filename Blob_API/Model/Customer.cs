using System;
using System.Collections.Generic;

namespace Blob_API.Model
{
    public partial class Customer
    {
        public Customer()
        {
            Order = new HashSet<Order>();
        }

        public uint Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime? CreatedAt { get; set; }
        public uint AddressId { get; set; }

        public virtual Address Address { get; set; }
        public virtual ICollection<Order> Order { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Blob_API.Model
{
    public partial class OrderedCustomer
    {
        public OrderedCustomer()
        {
            Order = new HashSet<Order>();
        }

        public uint Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public uint OrderedAddressId { get; set; }

        public virtual OrderedAddress OrderedAddress { get; set; }
        public virtual ICollection<Order> Order { get; set; }
    }
}

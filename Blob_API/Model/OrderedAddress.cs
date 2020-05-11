using System;
using System.Collections.Generic;

namespace Blob_API.Model
{
    public partial class OrderedAddress
    {
        public OrderedAddress()
        {
            OrderedCustomer = new HashSet<OrderedCustomer>();
        }

        public uint Id { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }

        public virtual ICollection<OrderedCustomer> OrderedCustomer { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Blob_API.Model
{
    public partial class Order
    {
        public Order()
        {
            OrderedProductOrder = new HashSet<OrderedProductOrder>();
        }

        public uint Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        [JsonIgnore]
        public uint CustomerId { get; set; }
        [JsonIgnore]
        public uint OrderedCustomerId { get; set; }
        [JsonIgnore]
        public uint StateId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual OrderedCustomer OrderedCustomer { get; set; }
        public virtual State State { get; set; }
        public virtual ICollection<OrderedProductOrder> OrderedProductOrder { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Blob_API.Model
{
    [DataContract]
    public partial class Order
    {
        public Order()
        {
            OrderedProductOrder = new HashSet<OrderedProductOrder>();
        }

        [DataMember]
        public uint Id { get; set; }
        [DataMember]
        public DateTime? CreatedAt { get; set; }
        [DataMember]
        public uint CustomerId { get; set; }
        [DataMember]
        public uint OrderedCustomerId { get; set; }
        [DataMember]
        public uint StateId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual OrderedCustomer OrderedCustomer { get; set; }
        public virtual State State { get; set; }
        public virtual ICollection<OrderedProductOrder> OrderedProductOrder { get; set; }
    }
}

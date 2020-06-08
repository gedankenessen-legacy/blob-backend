using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

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
        public uint? CustomerId { get; set; }
        [DataMember]
        public uint OrderedCustomerId { get; set; }
        [DataMember]
        public uint StateId { get; set; }


        [JsonIgnore]
        public virtual ICollection<OrderedProductOrder> OrderedProductOrder { get; set; }
        [JsonIgnore]
        public virtual Customer Customer { get; set; }
        [JsonIgnore]
        public virtual OrderedCustomer OrderedCustomer { get; set; }
        [JsonIgnore]
        public virtual State State { get; set; }
    }
}

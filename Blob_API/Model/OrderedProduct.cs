using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Blob_API.Model
{
    [DataContract]
    public partial class OrderedProduct
    {
        public OrderedProduct()
        {
            OrderedProductOrder = new HashSet<OrderedProductOrder>();
        }

        [DataMember]
        public uint Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public string Sku { get; set; }
        [DataMember]
        public uint ProductId { get; set; }

        [JsonIgnore]
        public virtual Product Product { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderedProductOrder> OrderedProductOrder { get; set; }
    }
}

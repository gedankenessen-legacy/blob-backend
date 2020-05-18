using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Blob_API.Model
{
    [DataContract]
    public partial class Product
    {
        public Product()
        {
            CategoryProduct = new HashSet<CategoryProduct>();
            LocationProduct = new HashSet<LocationProduct>();
            OrderedProduct = new HashSet<OrderedProduct>();
            ProductProperty = new HashSet<ProductProperty>();
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
        public DateTime? CreatedAt { get; set; }

        [JsonIgnore]
        public virtual ICollection<CategoryProduct> CategoryProduct { get; set; }
        [JsonIgnore]
        public virtual ICollection<LocationProduct> LocationProduct { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderedProduct> OrderedProduct { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductProperty> ProductProperty { get; set; }
    }
}

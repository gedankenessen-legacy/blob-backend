using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

        public virtual ICollection<CategoryProduct> CategoryProduct { get; set; }
        public virtual ICollection<LocationProduct> LocationProduct { get; set; }
        public virtual ICollection<OrderedProduct> OrderedProduct { get; set; }
        public virtual ICollection<ProductProperty> ProductProperty { get; set; }
    }
}

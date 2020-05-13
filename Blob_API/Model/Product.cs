using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Blob_API.Model
{
    public partial class Product
    {
        public Product()
        {
            CategoryProduct = new HashSet<CategoryProduct>();
            LocationProduct = new HashSet<LocationProduct>();
            OrderedProduct = new HashSet<OrderedProduct>();
            ProductProperty = new HashSet<ProductProperty>();
        }

        public uint Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Sku { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<CategoryProduct> CategoryProduct { get; set; }
        public virtual ICollection<LocationProduct> LocationProduct { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderedProduct> OrderedProduct { get; set; }
        public virtual ICollection<ProductProperty> ProductProperty { get; set; }
    }
}

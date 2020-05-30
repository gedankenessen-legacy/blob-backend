using System;
using System.Collections.Generic;
using Blob_API.Model;

namespace Blob_API.RessourceModels
{
    public class ProductRessource
    {
        public ProductRessource() { }

        public uint Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Sku { get; set; }
        public DateTime? CreatedAt { get; set; }
        public ICollection<Property> Properties { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<LocationProduct> ProductsAtLocations { get; set; }
    }
}

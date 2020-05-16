using System;
using System.Collections.Generic;
using Blob_API.Model;

namespace Blob_API.RessourceModels
{
    public class OrderRessource
    {
        public OrderRessource() { }

        public uint Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Customer Customer { get; set; } // TODO: CustomerRessource class

        public ICollection<OrderedProduct> OrderedProducts { get; set; } // TODO: OrderedProductsRessource class
    }
}
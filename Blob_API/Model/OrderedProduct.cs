using System;
using System.Collections.Generic;

namespace Blob_API.Model
{
    public partial class OrderedProduct
    {
        public OrderedProduct()
        {
            OrderedProductOrder = new HashSet<OrderedProductOrder>();
        }

        public uint Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Sku { get; set; }
        public uint ProductId { get; set; }

        public virtual Product Product { get; set; }
        public virtual ICollection<OrderedProductOrder> OrderedProductOrder { get; set; }
    }
}

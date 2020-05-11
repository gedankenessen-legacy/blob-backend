using System;
using System.Collections.Generic;

namespace Blob_API.Model
{
    public partial class OrderedProductOrder
    {
        public uint OrderedProductId { get; set; }
        public uint OrderId { get; set; }
        public uint Quantity { get; set; }

        public virtual Order Order { get; set; }
        public virtual OrderedProduct OrderedProduct { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Blob_API.Model
{
    public partial class LocationProduct
    {
        public uint LocationId { get; set; }
        public uint ProductId { get; set; }
        public uint Quantity { get; set; }

        public virtual Location Location { get; set; }
        public virtual Product Product { get; set; }
    }
}

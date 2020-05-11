using System;
using System.Collections.Generic;

namespace Blob_API.Model
{
    public partial class ProductProperty
    {
        public uint ProductId { get; set; }
        public uint PropertyId { get; set; }

        public virtual Product Product { get; set; }
        public virtual Property Property { get; set; }
    }
}

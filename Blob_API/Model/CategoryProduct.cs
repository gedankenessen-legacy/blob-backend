using System;
using System.Collections.Generic;

namespace Blob_API.Model
{
    public partial class CategoryProduct
    {
        public uint CategoryId { get; set; }
        public uint ProductId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Product Product { get; set; }
    }
}

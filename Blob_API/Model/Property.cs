using System;
using System.Collections.Generic;

namespace Blob_API.Model
{
    public partial class Property
    {
        public Property()
        {
            ProductProperty = new HashSet<ProductProperty>();
        }

        public uint Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public virtual ICollection<ProductProperty> ProductProperty { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Blob_API.Model
{
    public partial class Category
    {
        public Category()
        {
            CategoryProduct = new HashSet<CategoryProduct>();
        }

        public uint Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CategoryProduct> CategoryProduct { get; set; }
    }
}

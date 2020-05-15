using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Blob_API.Model
{
    [DataContract]
    public partial class Category
    {
        public Category()
        {
            CategoryProduct = new HashSet<CategoryProduct>();
        }

        [DataMember]
        public uint Id { get; set; }
        [DataMember]
        public string Name { get; set; }

        public virtual ICollection<CategoryProduct> CategoryProduct { get; set; }
    }
}

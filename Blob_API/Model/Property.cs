using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Blob_API.Model
{
    [DataContract]
    public partial class Property
    {
        public Property()
        {
            ProductProperty = new HashSet<ProductProperty>();
        }

        [DataMember]
        public uint Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Value { get; set; }

        public virtual ICollection<ProductProperty> ProductProperty { get; set; }
    }
}

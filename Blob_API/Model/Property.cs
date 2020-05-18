using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public virtual ICollection<ProductProperty> ProductProperty { get; set; }
    }
}

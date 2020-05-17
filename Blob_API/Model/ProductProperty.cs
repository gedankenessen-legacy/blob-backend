using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Blob_API.Model
{
    [DataContract]
    public partial class ProductProperty
    {
        [DataMember]
        public uint ProductId { get; set; }
        [DataMember]
        public uint PropertyId { get; set; }

        [JsonIgnore]
        public virtual Product Product { get; set; }
        [JsonIgnore]
        public virtual Property Property { get; set; }
    }
}

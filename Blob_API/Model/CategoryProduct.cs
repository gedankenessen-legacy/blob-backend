using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Blob_API.Model
{
    [DataContract]
    public partial class CategoryProduct
    {
        [DataMember]
        public uint CategoryId { get; set; }
        [DataMember]
        public uint ProductId { get; set; }

        [JsonIgnore]
        public virtual Category Category { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
    }
}

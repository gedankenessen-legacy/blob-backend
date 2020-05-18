using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Blob_API.Model
{
    [DataContract]
    public partial class OrderedProductOrder
    {
        [JsonIgnore]
        public uint OrderedProductId { get; set; }
        [JsonIgnore]
        public uint OrderId { get; set; }
        [DataMember]
        public uint Quantity { get; set; }

        [JsonIgnore]
        public virtual Order Order { get; set; }
        [DataMember]
        public virtual OrderedProduct OrderedProduct { get; set; }
    }
}

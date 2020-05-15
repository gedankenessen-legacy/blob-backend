using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Blob_API.Model
{
    [DataContract]
    public partial class OrderedProductOrder
    {
        [DataMember]
        public uint OrderedProductId { get; set; }
        [DataMember]
        public uint OrderId { get; set; }
        [DataMember]
        public uint Quantity { get; set; }

        public virtual Order Order { get; set; }
        public virtual OrderedProduct OrderedProduct { get; set; }
    }
}

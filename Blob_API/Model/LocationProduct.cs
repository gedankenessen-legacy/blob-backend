using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Blob_API.Model
{
    [DataContract]
    public partial class LocationProduct
    {
        [DataMember]
        public uint LocationId { get; set; }
        [DataMember]
        public uint ProductId { get; set; }
        [DataMember]
        public uint Quantity { get; set; }

        public virtual Location Location { get; set; }
        public virtual Product Product { get; set; }
    }
}

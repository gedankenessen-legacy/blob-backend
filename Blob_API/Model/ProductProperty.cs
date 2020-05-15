using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Blob_API.Model
{
    [DataContract]
    public partial class ProductProperty
    {
        [DataMember]
        public uint ProductId { get; set; }
        [DataMember]
        public uint PropertyId { get; set; }

        public virtual Product Product { get; set; }
        public virtual Property Property { get; set; }
    }
}

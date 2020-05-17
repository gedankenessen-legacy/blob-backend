using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Blob_API.Model
{
    [DataContract]
    public partial class State
    {
        public State()
        {
            Order = new HashSet<Order>();
        }

        [DataMember]
        public uint Id { get; set; }
        [DataMember]
        public string Value { get; set; }

        [JsonIgnore]
        public virtual ICollection<Order> Order { get; set; }
    }
}

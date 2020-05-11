using System;
using System.Collections.Generic;

namespace Blob_API.Model
{
    public partial class State
    {
        public State()
        {
            Order = new HashSet<Order>();
        }

        public uint Id { get; set; }
        public string Value { get; set; }

        public virtual ICollection<Order> Order { get; set; }
    }
}

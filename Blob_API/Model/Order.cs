using System;
using System.Collections.Generic;

namespace Blob_API.Model
{
    public partial class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Test { get; set; }
    }
}

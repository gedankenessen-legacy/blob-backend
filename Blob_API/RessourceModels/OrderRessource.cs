using System;
using System.Collections.Generic;
using Blob_API.Model;

namespace Blob_API.RessourceModels
{
    public class OrderRessource
    {
        public uint Id { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual CustomerRessource Customer { get; set; }
        public virtual OrderedCustomer OrderedCustomer { get; set; }
        public virtual State State { get; set; }
        public virtual ICollection<OrderedProductOrder> OrderedProductOrder { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Blob_API.Model;

namespace Blob_API.RessourceModels
{
    public class OrderRessource
    {
        public OrderRessource() { }

        public uint Id { get; set; }
        //public uint LocationId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public CustomerRessource Customer { get; set; }
        public ICollection<OrderedProductRessource> OrderedProducts { get; set; }
        public State State { get; set; }
    }
}
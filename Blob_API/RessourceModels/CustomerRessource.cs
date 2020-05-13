using System;
using System.Collections.Generic;
using Blob_API.Model;

namespace Blob_API.RessourceModels
{
    public class CustomerRessource
    {
        public uint Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Address Address { get; set; }
        //public virtual ICollection<OrderRessource> Order { get; set; }
    }
}

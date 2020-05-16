using System;
using Blob_API.Model;

namespace Blob_API.RessourceModels
{
    public class CustomerRessource
    {
        public CustomerRessource()
        {
        }

        public uint Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public virtual Address Address { get; set; } // TODO: AddressRessource class
        //public DateTime? CreatedAt { get; set; }
    }
}

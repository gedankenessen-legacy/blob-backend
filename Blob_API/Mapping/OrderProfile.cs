using System;
using AutoMapper;
using Blob_API.Model;
using Blob_API.RessourceModels;

namespace Blob_API.Mapping
{
    public class OrderProfile: Profile
    {
        public OrderProfile()
        {
            // Define the orgin and destination model for the mapping process.
            CreateMap<Order, OrderRessource>();
        }
    }
}

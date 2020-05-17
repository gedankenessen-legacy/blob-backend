using System;
using System.Linq;
using AutoMapper;
using Blob_API.Model;
using Blob_API.RessourceModels;

namespace Blob_API.RessourceMapping
{
    public class OrderedProductProfile : Profile
    {
        public OrderedProductProfile()
        {
            // Define the orgin and destination model for the mapping process.
            CreateMap<OrderedProduct, OrderedProductRessource>()
                //.ForMember(
                //dest => dest.Quantity,
                //opt => opt.MapFrom(
                //    src => src.OrderedProductOrder
                //        .Where(orderedProductOrder => orderedProductOrder.OrderedProductId == src.Id)
                .ReverseMap();
        }
    }
}

/*


    .Where(orderedProductOrder => orderedProductOrder.OrderedProductId == src.Id && orderedProductOrder.OrderedProduct == src)
                        .Select(x => x.Quantity).First()))
     */

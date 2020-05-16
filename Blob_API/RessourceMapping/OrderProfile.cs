using System.Linq;
using AutoMapper;
using Blob_API.Model;

namespace Blob_API.RessourceModels
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            // Define the orgin and destination model for the mapping process.
            CreateMap<Order, OrderRessource>()
                // OrderedProductOrder.OrderedProduct auf OrderedProducts mappen.
                .ForMember(dest => dest.OrderedProducts, opt => opt.MapFrom(src => src.OrderedProductOrder.Select(x => x.OrderedProduct).ToList()))
                .ReverseMap();
                    
        }
    }
}
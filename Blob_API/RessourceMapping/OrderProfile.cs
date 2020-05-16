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
                .ForMember(dest => dest.OrderedProducts, opt => opt.MapFrom(src => src.OrderedProductOrder));
        }
    }
}
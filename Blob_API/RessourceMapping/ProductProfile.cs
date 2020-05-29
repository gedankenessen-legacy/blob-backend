using System;
using System.Linq;
using AutoMapper;
using Blob_API.Model;
using Blob_API.RessourceModels;

namespace Blob_API.RessourceMapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // Define the orgin and destination model for the mapping process.
            CreateMap<Product, ProductRessource>()
                // ProductProperty.Property auf Properties mappen.
                .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => src.ProductProperty.Select(x => x.Property).ToList()))
                .ForMember(dest => dest.ProductsAtLocations, opt => opt.MapFrom(src => src.LocationProduct))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.CategoryProduct.Select(x => x.Category).ToList()))
                .ReverseMap();

        }
    }
}

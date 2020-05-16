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
                // CategoryProduct.Category auf Categories mappen.
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.CategoryProduct.Select(x => x.Category).ToList()));
                // LocationProduct auf locationProducts mappen.
                // TODO: LocationProductRessource class
                //.ForMember(dest => dest.locationProducts, opt => opt.MapFrom(src => src.LocationProduct));
        }
    }
}

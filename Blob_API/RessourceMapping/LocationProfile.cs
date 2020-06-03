using System;
using AutoMapper;
using Blob_API.Model;
using Blob_API.RessourceModels;

namespace Blob_API.RessourceMapping
{
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            // Define the orgin and destination model for the mapping process.
            CreateMap<Location, LocationRessource>()
                .ReverseMap();
        }
    }
}

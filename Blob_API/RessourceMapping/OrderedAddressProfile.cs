using AutoMapper;
using Blob_API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blob_API.RessourceMapping
{
    public class OrderedAddressProfile : Profile
    {
        public OrderedAddressProfile()
        {
            CreateMap<OrderedAddress, Address>()
                .ForMember(x => x.Customer, opt => opt.Ignore())    // Ignore Customer property
                .ForMember(x => x.Location, opt => opt.Ignore())    // Ignore location property
                .ReverseMap();
        }
    }
}

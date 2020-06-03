using AutoMapper;
using Blob_API.AuthModel;
using Blob_API.RessourceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blob_API.RessourceMapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserRessource>();
        }
    }
}

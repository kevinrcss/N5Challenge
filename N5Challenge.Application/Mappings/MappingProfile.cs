using AutoMapper;
using N5Challenge.Application.DTOs;
using N5Challenge.Core.Entities;

namespace N5Challenge.Application.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Permission, PermissionDto>().ReverseMap();
            CreateMap<PermissionType, PermissionTypeDto>().ReverseMap();
        }
    }
}

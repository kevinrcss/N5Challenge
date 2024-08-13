using AutoMapper;
using N5Challenge.Application.DTOs.Permission;
using N5Challenge.Application.DTOs.PermissionType;
using N5Challenge.Core.Entities;

namespace N5Challenge.Application.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Permission, PermissionDto>().ReverseMap();
            CreateMap<PermissionCreateDto, Permission>();
            CreateMap<PermissionType, PermissionTypeDto>().ReverseMap();
        }
    }
}

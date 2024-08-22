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
            //CreateMap<Permission, PermissionDto>().ReverseMap();

            CreateMap<Permission, PermissionDto>()
                .ForMember(dest => dest.PermissionTypeDescription, opt => opt.MapFrom(src => src.PermissionType.Description));

            //CreateMap<PermissionDto, Permission>()
            //    .ForMember(dest => dest.PermissionType, opt => opt.Ignore());

            CreateMap<PermissionCreateDto, Permission>();
            CreateMap<PermissionUpdateDto, Permission>();
            CreateMap<PermissionType, PermissionTypeDto>().ReverseMap();
        }
    }
}

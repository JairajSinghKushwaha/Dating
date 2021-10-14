using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>().ForMember(
            destination => destination.PhotoUrl,
            option => option.MapFrom(source => source.Photos.FirstOrDefault().Url))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<MemberUpdateDto, AppUser>(); // mapping in reverse order is possible
            CreateMap<Photo, PhotoDto>();
        }
    }
}
using AutoMapper;
using M241.Server.Common.Dtos;
using M241.Server.Data.Models;
using System;

namespace M241.Server.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RoomData, RoomDataDto>().ReverseMap();
        }
    }
}

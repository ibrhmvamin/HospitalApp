using AutoMapper;
using Business.Dtos.AppointmentDto;
using Business.Dtos.RoomDto;
using Business.Dtos.UserDtos;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppointmentCreateDto, Appointment>();
            CreateMap<AppUser, UserReturnDto>();
            CreateMap<Appointment, AppointmentReturnDto>().ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<AppUser, DoctorReturnDto>();
            CreateMap<Appointment, DoctorSchdelueReturnDto>();
            CreateMap<Message, MessageReturnDto>();
        }
    }
}

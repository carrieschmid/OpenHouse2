using AutoMapper;
using Domain;

namespace Application.Sessions {
    public class MappingProfile : Profile {
        public MappingProfile () {
            CreateMap<Session, SessionDto> ();
            CreateMap<UserSession, AttendeeDto> ()
                .ForMember (d => d.Username, o => o.MapFrom (s => s.AppUser.UserName))
                .ForMember (d => d.DisplayName, o => o.MapFrom (s => s.AppUser.DisplayName));
        }

    }
}
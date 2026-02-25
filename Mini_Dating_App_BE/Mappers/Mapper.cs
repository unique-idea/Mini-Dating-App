using AutoMapper;
using Mini_Dating_App_BE.Data.Models;
using Mini_Dating_App_BE.DTOs.Responses;
using Mini_Dating_App_BE.DTOs.Requests;

namespace Mini_Dating_App_BE.Mappers
{
    public class Mapper : Profile
    {
        public Mapper()
        {

            CreateMap<User, UserRes>()
              .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
              .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
              .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
              .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower()));


            CreateMap<RegisterReq, User>()
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
              .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
              .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
              .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<SetUserAvailabilityReq, Availability>()
             .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
             .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
             .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date));


            CreateMap<Availability, SetUserAvailabilityRes>()
             .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
             .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
             .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date));

            CreateMap<Match, UserMatchRes>()
           .ForMember(dest => dest.MatchId, opt => opt.MapFrom(src => src.MatchId))
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Match, ScheduleMatchRes>()
           .ForMember(dest => dest.MatchId, opt => opt.MapFrom(src => src.MatchId))
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<ScheduledDate, ScheduledDateRes>()
           .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
           .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
           .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime));


        }


    }
}

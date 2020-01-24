namespace Application.Activities
{
    using System.Linq;
    using AutoMapper;
    using Domain;
    using DTOs;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Activity, ActivityDto>();
            CreateMap<UserActivity, AttendeeDto>()
                .ForMember(dist => dist.Username, opts => opts.MapFrom(source => source.AppUser.UserName))
                .ForMember(dist => dist.DisplayName, opts => opts.MapFrom(source => source.AppUser.DisplayName))
                .ForMember(dist => dist.Image, opts => opts.MapFrom(source => source.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url));
                
        }
    }
}
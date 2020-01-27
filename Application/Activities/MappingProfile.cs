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
                .ForMember(dist => dist.Username, opts => opts.MapFrom(src => src.AppUser.UserName))
                .ForMember(dist => dist.DisplayName, opts => opts.MapFrom(src => src.AppUser.DisplayName))
                .ForMember(dist => dist.Image, opts => opts.MapFrom(src => src.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dist => dist.Following, opts => opts.MapFrom<FollowingResolver>());
        }
    }
}
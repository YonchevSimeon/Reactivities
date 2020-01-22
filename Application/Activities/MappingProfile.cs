namespace Application.Activities
{
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
                .ForMember(dist => dist.DisplayName, opts => opts.MapFrom(source => source.AppUser.DisplayName));
                
        }
    }
}
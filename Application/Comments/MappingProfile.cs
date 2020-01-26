namespace Application.Comments
{
    using System.Linq;
    using AutoMapper;
    using Domain;
    using DTOs;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.Username, opts => opts.MapFrom(src => src.Author.UserName))
                .ForMember(dest => dest.DisplayName, opts => opts.MapFrom(src => src.Author.DisplayName))
                .ForMember(dest => dest.Image, opts => opts.MapFrom(src => src.Author.Photos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}
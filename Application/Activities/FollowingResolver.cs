namespace Application.Activities
{
    using DTOs;
    using AutoMapper;
    using Domain;
    using Persistence;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    public class FollowingResolver : IValueResolver<UserActivity, AttendeeDto, bool>
    {
        private readonly DataContext context;
        private readonly IUserAccessor userAccessor;

        public FollowingResolver(DataContext context, IUserAccessor userAccessor)
        {
            this.context = context;
            this.userAccessor = userAccessor;
        }
        public bool Resolve(UserActivity source, AttendeeDto destination, bool destMember, ResolutionContext context)
        {
            AppUser currentUser = this.context.Users.SingleOrDefaultAsync(x => x.UserName == this.userAccessor.GetCurrentUsername()).Result;

            if (currentUser.Followings.Any(x => x.TargetId == source.AppUserId)) return true;

            return false;
        }
    }
}
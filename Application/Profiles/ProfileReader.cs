namespace Application.Profiles
{
    using System.Threading.Tasks;
    using Domain;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Persistence;
    using Errors;
    using System.Net;
    using System.Linq;

    public class ProfileReader : IProfileReader
    {
        private readonly DataContext context;
        private readonly IUserAccessor userAccessor;

        public ProfileReader(DataContext context, IUserAccessor userAccessor)
        {
            this.context = context;
            this.userAccessor = userAccessor;
        }
        public async Task<Profile> ReadProfile(string username)
        {
            AppUser user = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == username);

            if (user is null) throw new RestException(HttpStatusCode.NotFound, new { User = "Not found" });

            AppUser currentUser = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == this.userAccessor.GetCurrentUsername());

            Profile profile = new Profile
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                Image = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                Photos = user.Photos,
                Bio = user.Bio,
                FollowersCount = user.Followers.Count(),
                FollowingCount = user.Followings.Count()
            };

            if (currentUser.Followings.Any(x => x.TargetId == user.Id)) profile.IsFollowed = true;

            return profile;
        }
    }
}
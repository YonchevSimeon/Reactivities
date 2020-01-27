namespace Application.Followers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Persistence;
    using Profiles;

    public class List
    {
        public class Query : IRequest<List<Profile>> 
        {
            public string Username { get; set; }

            public string Predicate { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<Profile>>
        {
            private readonly DataContext context;
            private readonly IProfileReader profileReader;

            public Handler(DataContext context, IProfileReader profileReader)
            {
                this.context = context;
                this.profileReader = profileReader;
            }

            public async Task<List<Profile>> Handle(Query request, CancellationToken cancellationToken)
            {
                IQueryable<UserFollowing> queryable = this.context.Followings.AsQueryable();

                List<UserFollowing> userFollowings = new List<UserFollowing>();
                List<Profile> profiles= new List<Profile>();

                switch(request.Predicate)
                {
                    case "followers":
                    {
                        userFollowings = await queryable.Where(x => x.Target.UserName == request.Username).ToListAsync();

                        foreach(UserFollowing follower in userFollowings)
                            profiles.Add(await this.profileReader.ReadProfile(follower.Observer.UserName));

                        break;
                    }
                    case "following":
                    {
                        userFollowings = await queryable.Where(x => x.Observer.UserName == request.Username).ToListAsync();

                        foreach(UserFollowing follower in userFollowings)
                            profiles.Add(await this.profileReader.ReadProfile(follower.Target.UserName));

                        break;
                    }
                }

                return profiles;
            }
        }
    }
}
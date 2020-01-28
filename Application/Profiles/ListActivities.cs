namespace Application.Profiles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Errors;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Persistence;
    using DTOs;
    using Domain;

    public class ListActivities
    {
        public class Query : IRequest<List<UserActivityDto>>
        {
            public string Username { get; set; }
            public string Predicate { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<UserActivityDto>>
        {
            private readonly DataContext context;

            public Handler(DataContext context)
            {
                this.context = context;
            }

            public async Task<List<UserActivityDto>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                AppUser user = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == request.Username);

                if (user is null)
                    throw new RestException(HttpStatusCode.NotFound, new { User = "Not found" });

                IQueryable<UserActivity> queryable = user.UserActivities.OrderBy(a => a.Activity.Date).AsQueryable();

                switch (request.Predicate)
                {
                    case "past":
                        queryable = queryable.Where(a => a.Activity.Date <= DateTime.Now);
                        break;
                    case "hosting":
                        queryable = queryable.Where(a => a.IsHost);
                        break;
                    default:
                        queryable = queryable.Where(a => a.Activity.Date >= DateTime.Now);
                        break;
                }

                List<UserActivity> activities = queryable.ToList();
                List<UserActivityDto> activitiesToReturn = new List<UserActivityDto>();

                foreach (UserActivity activity in activities)
                {
                    UserActivityDto userActivity = new UserActivityDto
                    {
                        Id = activity.Activity.Id,
                        Title = activity.Activity.Title,
                        Category = activity.Activity.Category,
                        Date = activity.Activity.Date
                    };

                    activitiesToReturn.Add(userActivity);
                }

                return activitiesToReturn;
            }
        }
    }
}
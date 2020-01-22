namespace Application.Activities
{
    using Errors;
    using Interfaces;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Persistence;
    using Domain;
    using System.Net;
    using Microsoft.EntityFrameworkCore;

    public class Attend
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext context;
            private readonly IUserAccessor userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                this.context = context;
                this.userAccessor = userAccessor;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                Activity activity = 
                    await this.context.Activities
                        .FindAsync(request.Id);

                if (activity is null)
                    throw new RestException(HttpStatusCode.NotFound,
                        new { Activity = "Could not find activity" });

                AppUser user =
                    await this.context.Users
                        .SingleOrDefaultAsync(x => x.UserName == this.userAccessor.GetCurrentUsername());
                
                UserActivity attendance = 
                    await this.context.UserActivities
                        .SingleOrDefaultAsync(x => x.ActivityId == activity.Id && x.AppUserId == user.Id);
                
                if(attendance != null)
                    throw new RestException(HttpStatusCode.BadRequest,
                        new {Attendance = "Already attending this activity"});

                attendance = new UserActivity
                {
                    Activity = activity,
                    AppUser = user,
                    IsHost = false,
                    DateJoined = DateTime.Now
                };

                this.context.UserActivities.Add(attendance);

                bool success = await this.context.SaveChangesAsync() > 0;

                if (success) return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}
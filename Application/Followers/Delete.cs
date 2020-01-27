namespace Application.Followers
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using Errors;
    using Interfaces;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Persistence;

    public class Delete
    {
        public class Command : IRequest
        {
            public string Username { get; set; }
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
                AppUser observer = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == this.userAccessor.GetCurrentUsername());

                AppUser target = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == request.Username);

                if (target is null) throw new RestException(HttpStatusCode.NotFound, new { User = "Not found" });

                UserFollowing following = await this.context.Followings.SingleOrDefaultAsync(x => x.ObserverId == observer.Id && x.TargetId == target.Id);

                if (following is null) throw new RestException(HttpStatusCode.BadRequest, new { User = "You are not following this user" });

                if (following != null)
                {
                    this.context.Followings.Remove(following);
                }

                bool success = await this.context.SaveChangesAsync() > 0;

                if (success) return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}
namespace Application.Followers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Persistence;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Domain;
    using Errors;
    using System.Net;

    public class Add
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

                if(following != null) throw new RestException(HttpStatusCode.BadRequest, new {User = "You are already following this user"});

                if(following is null)
                {
                    following = new UserFollowing
                    {
                        Observer = observer,
                        Target = target
                    };

                    this.context.Followings.Add(following);
                }

                bool success = await this.context.SaveChangesAsync() > 0;

                if (success) return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}
namespace Application.Photos
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Persistence;
    using Interfaces;
    using Domain;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using Errors;
    using System.Net;
    
    public class SetMain
    {
        public class Command : IRequest
        {
            public string Id { get; set; }
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
                AppUser user =
                    await this.context.Users.SingleOrDefaultAsync(x => x.UserName == this.userAccessor.GetCurrentUsername());

                Photo photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);

                if (photo is null) throw new RestException(HttpStatusCode.NotFound, new { Photo = "Not Found" });

                Photo currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

                currentMain.IsMain = false;
                photo.IsMain = true;

                bool success = await this.context.SaveChangesAsync() > 0;

                if (success) return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}
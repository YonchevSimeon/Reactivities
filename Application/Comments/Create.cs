namespace Application.Comments
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Persistence;
    using DTOs;
    using AutoMapper;
    using Errors;
    using System.Net;
    using Domain;
    using Microsoft.EntityFrameworkCore;

    public class Create
    {
        public class Command : IRequest<CommentDto>
        {
            public string Body { get; set; }

            public Guid ActivityId { get; set; }

            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Command, CommentDto>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }

            public async Task<CommentDto> Handle(Command request, CancellationToken cancellationToken)
            {
                Activity activity = await this.context.Activities.FindAsync(request.ActivityId);

                if (activity is null) throw new RestException(HttpStatusCode.NotFound, new { Activity = "Not found" });

                AppUser user = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == request.Username);

                Comment comment = new Comment
                {
                    Author = user,
                    Activity = activity,
                    Body = request.Body,
                    CreateAt = DateTime.Now
                };

                activity.Comments.Add(comment);

                bool success = await this.context.SaveChangesAsync() > 0;

                if (success) return this.mapper.Map<CommentDto>(comment);

                throw new Exception("Problem saving changes");
            }
        }
    }
}
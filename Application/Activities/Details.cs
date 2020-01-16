namespace Application.Activities
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using MediatR;
    using Persistence;
    using Errors;

    public class Details
    {
        public class Query : IRequest<Activity>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Activity>
        {
            private readonly DataContext context;

            public Handler(DataContext context)
            {
                this.context = context;
            }

            public async Task<Activity> Handle(Query request, CancellationToken cancellationToken)
            {
                Activity activity = await this.context.Activities.FindAsync(request.Id);

                if (activity is null) throw new RestException(HttpStatusCode.NotFound, new { activity = "Not found" });

                return activity;
            }
        }
    }
}
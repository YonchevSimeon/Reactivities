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
    using Microsoft.EntityFrameworkCore;
    using DTOs;
    using AutoMapper;

    public class Details
    {
        public class Query : IRequest<ActivityDto>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, ActivityDto>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }

            public async Task<ActivityDto> Handle(Query request, CancellationToken cancellationToken)
            {
                Activity activity = await this.context.Activities.FindAsync(request.Id);

                if (activity is null) throw new RestException(HttpStatusCode.NotFound, new { activity = "Not found" });

                return this.mapper.Map<Activity, ActivityDto>(activity);
            }
        }
    }
}
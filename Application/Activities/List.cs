namespace Application.Activities
{
    using DTOs;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Persistence;
    using AutoMapper;

    public class List
    {
        public class Query : IRequest<List<ActivityDto>> { }

        public class Handler : IRequestHandler<Query, List<ActivityDto>>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }

            public async Task<List<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                List<Activity> activities = await this.context.Activities.ToListAsync();

                return this.mapper.Map<List<Activity>, List<ActivityDto>>(activities);
            }
        }
    }
}
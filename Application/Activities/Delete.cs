namespace Application.Activities
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using MediatR;
    using Persistence;

    public class Delete
    {
        public class Command : IRequest
                {
                    public Guid Id { get; set; }
                }
        
                public class Handler : IRequestHandler<Command>
                {
                    private readonly DataContext context;
        
                    public Handler(DataContext context)
                    {
                        this.context = context;
                    }
        
                    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
                    {
                        Activity activity = await this.context.Activities.FindAsync(request.Id);

                        if(activity is null) throw new Exception("Could not find activity");

                        this.context.Remove(activity);

                        bool success = await this.context.SaveChangesAsync() > 0;
        
                        if(success) return Unit.Value;
        
                        throw new Exception("Problem saving changes");
                    }
                }
    }
}
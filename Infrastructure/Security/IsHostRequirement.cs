namespace Infrastructure.Security
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Domain;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Persistence;

    public class IsHostRequirement : IAuthorizationRequirement { }

    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly DataContext context;

        public IsHostRequirementHandler(IHttpContextAccessor httpContextAccessor, DataContext context)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            string currentUserName = 
                this.httpContextAccessor
                    .HttpContext
                    .User?
                    .Claims?
                    .SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?
                    .Value;

            Guid activityId = 
                Guid.Parse(this.httpContextAccessor
                    .HttpContext
                    .Request
                    .RouteValues
                    .SingleOrDefault(x => x.Key == "id")
                    .Value
                    .ToString());

            Activity activity = this.context.Activities.FindAsync(activityId).Result;

            UserActivity host = activity.UserActivities.FirstOrDefault(x => x.IsHost);

            if (host?.AppUser?.UserName == currentUserName)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
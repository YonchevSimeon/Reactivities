namespace Application.User
{
    using System.Threading;
    using System.Threading.Tasks;
    using Interfaces;
    using Domain;
    using MediatR;
    using Microsoft.AspNetCore.Identity;

    public class CurrentUser
    {
        public class Query : IRequest<User> { }

        public class Handler : IRequestHandler<Query, User>
        {
            private readonly UserManager<AppUser> userManager;
            private readonly IJwtGenerator jwtGenerator;
            private readonly IUserAccessor userAccessor;

            public Handler(UserManager<AppUser> userManager, IJwtGenerator jwtGenerator, IUserAccessor userAccessor)
            {
                this.userManager = userManager;
                this.jwtGenerator = jwtGenerator;
                this.userAccessor = userAccessor;
            }

            public async Task<User> Handle(Query request, CancellationToken cancellationToken)
            {
                AppUser appUser = await this.userManager.FindByNameAsync(this.userAccessor.GetCurrentUsername());

                User user = new User
                {
                    DisplayName = appUser.DisplayName,
                    Username = appUser.UserName,
                    Token = this.jwtGenerator.CreateToken(appUser),
                    Image = null
                };

                return user;
            }
        }
    }
}
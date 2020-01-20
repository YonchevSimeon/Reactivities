namespace Application.User
{
    using Interfaces;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using FluentValidation;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Errors;
    using System.Net;

    public class Login
    {
        public class Query : IRequest<User>
        {
            public string Email { get; set; }

            public string Password { get; set; }
        }

        public class QueryValiadtor : AbstractValidator<Query>
        {
            public QueryValiadtor()
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Query, User>
        {
            private readonly UserManager<AppUser> userManager;
            private readonly SignInManager<AppUser> signInManager;
            private readonly IJwtGenerator jwtGenerator;

            public Handler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtGenerator jwtGenerator)
            {
                this.userManager = userManager;
                this.signInManager = signInManager;
                this.jwtGenerator = jwtGenerator;
            }

            public async Task<User> Handle(Query request, CancellationToken cancellationToken)
            {
                AppUser user = await this.userManager.FindByEmailAsync(request.Email);

                if (user is null) throw new RestException(HttpStatusCode.Unauthorized);

                SignInResult result = await this.signInManager.CheckPasswordSignInAsync(user, request.Password, false);

                if (result.Succeeded)
                {
                    return new User
                    {
                        DisplayName = user.DisplayName,
                        Token = this.jwtGenerator.CreateToken(user),
                        Username = user.UserName,
                        Image = null
                    };
                }

                throw new RestException(HttpStatusCode.Unauthorized);
            }
        }
    }
}
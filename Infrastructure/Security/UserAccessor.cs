namespace Infrastructure.Security
{
    using System.Linq;
    using System.Security.Claims;
    using Application.Interfaces;
    using Microsoft.AspNetCore.Http;

    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUsername()
        {
            string username =
                this.httpContextAccessor
                    .HttpContext
                    .User?
                    .Claims?
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?
                    .Value;

            return username;
        }
    }
}
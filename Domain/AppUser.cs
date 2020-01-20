namespace Domain
{
    using Microsoft.AspNetCore.Identity;

    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
    }
}
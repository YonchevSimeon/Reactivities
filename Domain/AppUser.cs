namespace Domain
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;

    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }

        public string Bio { get; set; }

        public virtual ICollection<UserActivity> UserActivities { get; set; }

        public virtual ICollection<Photo> Photos { get; set; }
    }
}
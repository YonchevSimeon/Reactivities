namespace Application.Profiles
{
    using Domain;
    using System.Collections.Generic;

    public class Profile
    {
        public string DisplayName { get; set; }

        public string Username { get; set; }

        public string Image { get; set; }

        public string Bio { get; set; }

        public ICollection<Photo> Photos { get; set; }
    }
}
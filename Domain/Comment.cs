namespace Domain
{
    using System;

    public class Comment
    {
        public Guid Id { get; set; }

        public string Body { get; set; }
    
        public virtual AppUser Author { get; set; }

        public virtual Activity Activity { get; set; }
    
        public virtual DateTime CreateAt { get; set; }
    }
}
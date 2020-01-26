namespace Application.Comments.DTOs
{
    using System;

    public class CommentDto
    {
        public Guid Id { get; set; }

        public string Body { get; set; }
    
        public virtual DateTime CreateAt { get; set; }
    
        public string Username { get; set; }

        public string DisplayName { get; set; }

        public string Image { get; set; }
    }
}
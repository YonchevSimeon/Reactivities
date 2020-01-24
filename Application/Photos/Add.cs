namespace Application.Photos
{
    using Interfaces;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Persistence;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    public class Add
    {
        public class Command : IRequest<Photo>
        {
            public IFormFile file { get; set; }
        }

        public class Handler : IRequestHandler<Command, Photo>
        {
            private readonly DataContext context;
            private readonly IUserAccessor userAccessor;
            private readonly IPhotoAccessor photoAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor, IPhotoAccessor photoAccessor)
            {
                this.context = context;
                this.userAccessor = userAccessor;
                this.photoAccessor = photoAccessor;
            }

            public async Task<Photo> Handle(Command request, CancellationToken cancellationToken)
            {
                PhotoUploadResult photoUploadResult = this.photoAccessor.AddPhoto(request.file);

                AppUser user = 
                    await this.context.Users.SingleOrDefaultAsync(x => x.UserName == this.userAccessor.GetCurrentUsername());

                Photo photo = new Photo
                {
                    Id = photoUploadResult.PublicId,
                    Url = photoUploadResult.Url
                };

                if (!user.Photos.Any(x => x.IsMain))
                    photo.IsMain = true;

                user.Photos.Add(photo);

                bool success = await this.context.SaveChangesAsync() > 0;

                if (success) return photo;

                throw new Exception("Problem saving changes");
            }
        }
    }
}
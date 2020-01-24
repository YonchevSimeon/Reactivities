namespace Application.Interfaces
{
    using Photos;
    using Microsoft.AspNetCore.Http;

    public interface IPhotoAccessor
    {
        PhotoUploadResult AddPhoto(IFormFile file);

        string DeletePhoto(string publicId);
    }
}
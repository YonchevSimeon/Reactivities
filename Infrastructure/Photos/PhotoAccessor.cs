namespace Infrastructure.Photos
{
    using System;
    using System.IO;
    using Application.Interfaces;
    using Application.Photos;
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;

    public class PhotoAccessor : IPhotoAccessor
    {
        private readonly Cloudinary cloudinary;

        public PhotoAccessor(IOptions<CloudinarySettings> config)
        {
            Account account = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            this.cloudinary = new Cloudinary(account);
        }
        public PhotoUploadResult AddPhoto(IFormFile file)
        {
            ImageUploadResult uploadResult = new ImageUploadResult();

            if(file.Length > 0)
            {
                using (Stream stream = file.OpenReadStream())
                {
                    ImageUploadParams uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation()
                            .Height(500)
                            .Width(500)
                            .Crop("fill")
                            .Gravity("face")
                    };

                    uploadResult = this.cloudinary.Upload(uploadParams);
                }
            }

            if(uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }

            return new PhotoUploadResult
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUri.AbsoluteUri
            };
        }

        public string DeletePhoto(string publicId)
        {
            DeletionParams deleteParams = new DeletionParams(publicId);

            DeletionResult result = this.cloudinary.Destroy(deleteParams);

            return result.Result == "ok" ? result.Result : null;
        }
    }
}
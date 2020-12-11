using HiWorld.Data.Common.Repositories;
using HiWorld.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HiWorld.Services.Data
{
    public class ImagesService : IImagesService
    {
        private readonly IRepository<Image> imageRepository;

        public ImagesService(IRepository<Image> imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        public async Task<string> Create(IFormFile image, string path)
        {
            Directory.CreateDirectory($"{path}/");
            var extension = Path.GetExtension(image.FileName).TrimStart('.');

            var imageObj = new Image
            {
                Extension = extension,
            };

            await this.imageRepository.AddAsync(imageObj);
            await this.imageRepository.SaveChangesAsync();

            var physicalPath = $"{path}/{imageObj.Id}.{extension}";
            using (Stream fileStream = new FileStream(physicalPath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return imageObj.Id;
        }
    }
}

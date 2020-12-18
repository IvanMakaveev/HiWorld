namespace HiWorld.Services.Data
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface IImagesService
    {
        Task<string> CreateAsync(IFormFile image, string path);
    }
}
